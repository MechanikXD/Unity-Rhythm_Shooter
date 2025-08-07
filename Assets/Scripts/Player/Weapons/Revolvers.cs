using System;
using System.Collections;
using Core.Game;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    public class Revolvers : WeaponBase {
        private bool _walkAnimationSwitch;
        private Func<Vector3, Ray> _rayForward;

        private int _maxAmmoPerRevolver = 6;
        private int _leftCurrentAmmo;
        private int _rightCurrentAmmo;

        private bool _leftInAnimation;
        private bool _rightInAnimation;
        private bool _isWalking;
        private Action _unsubscribeFromEvents;
        
        public override void LeftPerfectAction() => 
            PerformAction(4, "Shoot Left", 1f / 3f, true);
        public override void LeftGoodAction() => 
            PerformAction(3, "Shoot Left", 1f / 3f, true);
        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, "Shoot Left", 1f / 3f, true);
        }

        public override void RightPerfectAction() => 
            PerformAction(4, "Shoot Right", 1f / 3f, false);
        public override void RightGoodAction() => 
            PerformAction(4, "Shoot Right", 1f / 3f, false);
        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, "Shoot Right", 1f / 3f, false);
        }
        
        private void PerformAction(int damage, string animationName, float animationTime, bool left) {
            if (left) {
                if (_leftInAnimation) return;
                _leftInAnimation = true;
            }
            else {
                if (_rightInAnimation) return;
                _rightInAnimation = true;
            }
            
            IEnumerator SetLeftNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
            }

            IEnumerator SetRightNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _rightInAnimation = false;
            }
            
            ShootForward(damage);
            _animator.Play(animationName, -1, 0f);
            GameManager.Instance.StartCoroutine(left
                ? SetLeftNotInAnimation(animationTime)
                : SetRightNotInAnimation(animationTime));
        }
        
        private void ShootForward(int damage) {
            if (Physics.Raycast(_rayForward(new Vector2(Screen.width / 2f, Screen.height / 2f)),
                    out var hit, _maxShootDistance) &&
                hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {
                
                damageable.TakeDamage(damage);
                PlayerEvents.OnDamageDealt();
            }
            else {
                PlayerEvents.OnAttackFailed();
            }
        }

        public override bool CanDoLeftAction() => !_leftInAnimation;

        public override bool CanDoRightAction() => !_rightInAnimation;

        public override bool CanDoBothAction() => false;
        public override void StartReload() {
            throw new NotImplementedException();
        }
        public override void FastReload() {
            throw new NotImplementedException();
        }
        public override void SlowReload() {
            throw new NotImplementedException();
        }

        public override void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");
            
            _rayForward = Camera.main!.ScreenPointToRay;
            
            _leftInAnimation = true;
            _rightInAnimation = true;
            _animator.Play("Selected", -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
                _rightInAnimation = false;
            }
            GameManager.Instance.StartCoroutine(SetNotInAnimation(0.6f));

            void SetIsWalking() => _isWalking = true;
            void SetNotWalking() => _isWalking = false;

            PlayerEvents.StartWalking += SetIsWalking;
            PlayerEvents.StoppedWalking += SetNotWalking;
            Conductor.Instance.AppendRepeatingAction("Weapon Walk", AnimateWalk);

            _unsubscribeFromEvents = () => {
                PlayerEvents.StartWalking -= SetIsWalking;
                PlayerEvents.StoppedWalking -= SetNotWalking;
            };
        }

        private void AnimateWalk() {
            if (!_isWalking) return;
            
            if (_walkAnimationSwitch) {
                if (!_leftInAnimation) _animator.Play("Walk Left", -1, 0f);
                _walkAnimationSwitch = false;
            }
            else {
                if (!_rightInAnimation) _animator.Play("Walk Right", -1, 0f);
                _walkAnimationSwitch = true;
            }
        }

        public override void OnWeaponDeselected() => _unsubscribeFromEvents();
    }
}