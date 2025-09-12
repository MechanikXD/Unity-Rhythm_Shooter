using System;
using System.Collections;
using Core.Behaviour.BehaviourInjection;
using Core.Game;
using Core.Music;
using Interactable.Damageable;
using Player.Weapons.Base;
using UI;
using UI.Views.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Weapons.Definitions {
    public class Shield : WeaponBase {
        [SerializeField] private ShieldAttackCollider _attackCollider;
        [SerializeField] private float _parryWindowDuration = 0.2f;
        [SerializeField] private PlayerInput _playerInput;
        private InputAction _blockAction;
        private float _currentBlockDuration;
        private DamageableBehaviour _player;

        private float _playerDefaultDamageReduction;
        [SerializeField] private float _blockAngle = 90f;
        [SerializeField] private float _passiveDamageReduction = 1f;
        [SerializeField] private float _shieldedDamageReduction = 0.6f;
        
        private BehaviourInjection<int> _leftActionBehaviour;
        private BehaviourInjection<float> _rightActionBehaviour;
        private ShieldView _relatedCanvas;

        private bool _wasBlockingLastFrame;
        private bool _canParry;
        private bool _isBlocking;

        [SerializeField] private AudioClip[] _blockedAttackSounds;
        
        private bool _inAnimation;
        private Action _unsubscribeFromEvents;
        private readonly static int IsBlocking = Animator.StringToHash("IsBlocking");
        
        public override void LeftPerfectAction() => _leftActionBehaviour.Perform(7);

        public override void LeftGoodAction() => _leftActionBehaviour.Perform(5);

        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            _leftActionBehaviour.Perform(3);
        }

        public override void RightPerfectAction() {
            _canParry = true;
            _rightActionBehaviour.Perform(_shieldedDamageReduction);
        }

        public override void RightGoodAction() => 
            _rightActionBehaviour.Perform(_shieldedDamageReduction * 1.2f);
        

        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            _rightActionBehaviour.Perform(_shieldedDamageReduction * 1.5f);
        }

        private void ShieldAttack(int damage) {
            if (!CanDoLeftAction()) return;
            
            _inAnimation = true;
            _attackCollider.ActivateCollider(damage);
            PlaySound(_shotSounds);
            
            IEnumerator SetNotInAnimation() {
                yield return new WaitForSeconds(HalfCrotchet);
                _inAnimation = false;
                _attackCollider.DeactivateCollider();
            }
            
            _animator.CrossFade("Attack", _crossFade, -1, 0f);
            StartCoroutine(SetNotInAnimation());
        }

        private void StartBlocking(float damageReduction) {
            if (!CanDoRightAction()) return;

            _relatedCanvas.SetShieldIconActive(true);
            _animator.CrossFade("Shielded", _crossFade);
            _isBlocking = true;
            _animator.SetBool(IsBlocking, _isBlocking);
            _player.DamageProcessor.ChangeBehaviour(ShieldedDamageProcessor);
        }

        private int ShieldedDamageProcessor(DamageInfo info) {
            if (IsEnemyInFront(_player.transform, info.SourcePosition, _blockAngle)) {
                PlaySound(_blockedAttackSounds);
                return _currentBlockDuration < _parryWindowDuration && _canParry 
                    ? 0 
                    : (int)(info.DamageValue - info.DamageValue *
                    (_player.CurrentDamageReduction - _shieldedDamageReduction));
            }

            return (int)(info.DamageValue - info.DamageValue * _player.CurrentDamageReduction);
        }
        
        private static bool IsEnemyInFront(Transform player, Vector3 enemyPosition, float maxAngle) {
            Vector3 playerForward = player.forward;
            playerForward.y = 0;  // Ignore Y
            playerForward.Normalize();
    
            Vector3 directionToEnemy = enemyPosition - player.position;
            directionToEnemy.y = 0;  // Ignore Y
            directionToEnemy.Normalize();
    
            float dot = Vector3.Dot(playerForward, directionToEnemy);
            float cosAngle = Mathf.Cos(maxAngle * 0.5f * Mathf.Deg2Rad);
    
            return dot >= cosAngle;
        }

        public override bool CanDoLeftAction() => !_isBlocking && !_inAnimation;

        public override bool CanDoRightAction() => !_inAnimation;

        public override bool CanDoBothAction() => false;
        
        // No Reload, Empty function body.
        public override void StartReload() { }
        public override void FastReload() { }
        public override void SlowReload() { }

        public override void OnWeaponSelected() {
            base.OnWeaponSelected();
            _attackCollider.DeactivateCollider();
            _currentBlockDuration = 0f;
            _blockAction = _playerInput.actions["RightAction"];
            _player = GameManager.Instance.Player;

            _relatedCanvas = UIManager.Instance.GetHUDCanvas<ShieldView>();
            _relatedCanvas.SetShieldIconActive(false);
            UIManager.Instance.EnterHUDCanvas<ShieldView>();
            
            _playerDefaultDamageReduction = _player.CurrentDamageReduction;
            _player.SetDamageReduction(_passiveDamageReduction);
            _leftActionBehaviour = new BehaviourInjection<int>(ShieldAttack);
            _rightActionBehaviour = new BehaviourInjection<float>(StartBlocking);
            
            _inAnimation = true;
            _animator.CrossFade("Selected", _crossFade, -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _inAnimation = false;
            }
            StartCoroutine(SetNotInAnimation(35f / 60f));
            
            void AnimateWalk() {
                if (IsWalking && !_inAnimation) 
                    _animator.CrossFade(_isBlocking ? "Walk Shielded" : "Walk", _crossFade, -1, 0f);
            } 
            
            void SetIsWalking() => IsWalking = true;
            void SetNotWalking() => IsWalking = false;

            PlayerEvents.StartWalking += SetIsWalking;
            PlayerEvents.StoppedWalking += SetNotWalking;
            Conductor.NextBeat += AnimateWalk;

            _unsubscribeFromEvents = () => {
                Conductor.NextBeat -= AnimateWalk;
                PlayerEvents.StartWalking -= SetIsWalking;
                PlayerEvents.StoppedWalking -= SetNotWalking;
            };
        }

        public override void WeaponUpdate() {
            if (_isBlocking && _blockAction.IsPressed()) {
                _wasBlockingLastFrame = true;
                _currentBlockDuration += Time.deltaTime;
            }
            else if (_wasBlockingLastFrame && !_blockAction.IsPressed()) {
                _animator.CrossFade("Idle", _crossFade);
                _animator.SetBool(IsBlocking, _isBlocking);
                _isBlocking = false;
                _canParry = false;
                _wasBlockingLastFrame = false;
                _player.DamageProcessor.ChangeToDefaultBehaviour();
                _relatedCanvas.SetShieldIconActive(false);
            }
        }

        public override void OnWeaponDeselected() {
            _unsubscribeFromEvents();
            UIManager.Instance.ExitHudCanvas<ShieldView>();
            _player.SetDamageReduction(_playerDefaultDamageReduction);
        }

        protected override void UpdateAnimationsSpeed() {
            _animator.SetFloat(WalkSpeed, _walk.length / Crotchet);
            _animator.SetFloat(ShootSpeed, _action.length / HalfCrotchet);
        }
    }
}