using System;
using System.Collections;
using Core.Game;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Weapons {
    public class Shield : WeaponBase {
        [SerializeField] private ShieldAttackCollider _attackCollider;
        [SerializeField] private float _parryWindowDuration = 0.2f;
        [SerializeField] private PlayerInput _playerInput;
        private InputAction _blockAction;
        private float _currentBlockDuration;

        [SerializeField] private float _passiveDamageReduction = 1f;
        [SerializeField] private float _shieldedDamageReduction = 0.6f;
        private float _currentDamageBlockingReduction;

        private bool _wasBlockingLastFrame;
        private bool _canParry;
        private bool _isBlocking;
        
        private bool _inAnimation;
        private Action _unsubscribeFromEvents;

        private void ShieldedPlayerHealthBehaviour(DamageInfo info) {
            if (_canParry && _currentBlockDuration <= _parryWindowDuration) {
                foreach (var target in info.Targets) target.Parried(info);
            }
            else if (_isBlocking) {
                info.Targets[0].TakeDirectDamage((int)(info.DamageValue * _currentDamageBlockingReduction));
            }
            else {
                info.Targets[0].TakeDirectDamage((int)(info.DamageValue * _passiveDamageReduction));
            }
        }
        
        public override void LeftPerfectAction() => ShieldAttack(7);

        public override void LeftGoodAction() => ShieldAttack(5);

        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            ShieldAttack(3);
        }

        public override void RightPerfectAction() {
            _canParry = true;
            StartBlocking(_shieldedDamageReduction);
        }

        public override void RightGoodAction() => 
            StartBlocking(_shieldedDamageReduction * 1.2f);

        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            StartBlocking(_shieldedDamageReduction * 1.5f);
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
            
            _animator.CrossFade("Shoot", _crossFade, -1, 0f);
            StartCoroutine(SetNotInAnimation());
        }

        private void StartBlocking(float damageReduction) {
            if (!CanDoRightAction()) return;

            _currentDamageBlockingReduction = damageReduction;
            _animator.CrossFade("Blocking Idle", _crossFade);
            _isBlocking = true;
        }

        public override bool CanDoLeftAction() => !_isBlocking && !_inAnimation;

        public override bool CanDoRightAction() => !_inAnimation;

        public override bool CanDoBothAction() => false;
        
        // No Reload, Empty function body.
        public override void StartReload() { }
        public override void FastReload() { }
        public override void SlowReload() { }

        public override void OnWeaponSelected() {
            HalfCrotchet = Conductor.Instance.SongData.HalfCrotchet;
            Crotchet = Conductor.Instance.SongData.Crotchet;
            
            _attackCollider.DeactivateCollider();
            _currentBlockDuration = 0f;
            _blockAction = _playerInput.actions["Right Action"];
            GameManager.Instance.Player.HealthBehaviour.ChangeBehaviour(ShieldedPlayerHealthBehaviour);
            base.CalculateAnimationsSpeed();
            
            _inAnimation = true;
            _animator.CrossFade("Selected", _crossFade, -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _inAnimation = false;
            }
            // TODO: Replace with selected animation time
            StartCoroutine(SetNotInAnimation(0.6f));
            
            void AnimateWalk() {
                if (IsWalking && !_inAnimation) 
                    _animator.CrossFade(_isBlocking ? "Blocking Walk" : "Walk", _crossFade, -1, 0f);
            } 
            
            void SetIsWalking() => IsWalking = true;
            void SetNotWalking() => IsWalking = false;

            PlayerEvents.StartWalking += SetIsWalking;
            PlayerEvents.StoppedWalking += SetNotWalking;
            Conductor.Instance.AppendRepeatingAction("Weapon Walk", AnimateWalk);

            _unsubscribeFromEvents = () => {
                PlayerEvents.StartWalking -= SetIsWalking;
                PlayerEvents.StoppedWalking -= SetNotWalking;
            };
        }

        public override void WeaponUpdate() {
            if (_isBlocking && _blockAction.IsPressed()) {
                _currentBlockDuration += Time.deltaTime;
            }
            else if (_wasBlockingLastFrame) {
                _animator.CrossFade("Idle", _crossFade);
                _isBlocking = false;
                _canParry = false;
            }
        }

        public override void OnWeaponDeselected() {
            _unsubscribeFromEvents();
            GameManager.Instance.Player.HealthBehaviour.ChangeToDefaultBehaviour();
        }
    }
}