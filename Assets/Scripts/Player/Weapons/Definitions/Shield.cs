using System;
using System.Collections;
using Core.Behaviour.BehaviourInjection;
using Core.Game;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
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

        [SerializeField] private float _passiveDamageReduction = 1f;
        [SerializeField] private float _shieldedDamageReduction = 0.6f;
        
        private BehaviourInjection<int> _leftActionBehaviour;
        private BehaviourInjection<float> _rightActionBehaviour;

        private bool _wasBlockingLastFrame;
        // TODO: Finish Parry Mechanic
        private bool _canParry;
        private bool _isBlocking;
        
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

            _player.SetDamageReduction(_player.CurrentDamageReduction - _passiveDamageReduction + _shieldedDamageReduction);
            _animator.CrossFade("Shielded", _crossFade);
            _isBlocking = true;
            _animator.SetBool(IsBlocking, _isBlocking);
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
            Conductor.Instance.AppendRepeatingAction("Walk", AnimateWalk);

            _unsubscribeFromEvents = () => {
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
                _player.SetDamageReduction(_player.CurrentDamageReduction -
                    _shieldedDamageReduction + _passiveDamageReduction);
            }
        }

        public override void OnWeaponDeselected() {
            _unsubscribeFromEvents();
        }

        protected override void CalculateAnimationsSpeed() {
            _animator.SetFloat(WalkSpeed, _walk.length / Crotchet);
            _animator.SetFloat(ShootSpeed, _action.length / HalfCrotchet);
        }
    }
}