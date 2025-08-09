using System;
using System.Collections;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    public class Revolvers : WeaponBase {
        private bool _walkAnimationSwitch;
        private Func<Vector3, Ray> _rayForward;
        
        private int _leftCurrentAmmo;
        private int _rightCurrentAmmo;

        private bool _leftInAnimation;
        private bool _rightInAnimation;

        private Coroutine _reloadRemoveInAnimation;
        private Action _unsubscribeFromEvents;

        public override void LeftPerfectAction() => 
            PerformAction(4, "Shoot Left", true);
        public override void LeftGoodAction() => 
            PerformAction(3, "Shoot Left", true);
        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, "Shoot Left", true);
        }

        public override void RightPerfectAction() => 
            PerformAction(4, "Shoot Right", false);
        public override void RightGoodAction() => 
            PerformAction(4, "Shoot Right", false);
        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, "Shoot Right", false);
        }
        
        private void PerformAction(int damage, string animationName, bool left) {
            if (left) {
                if (!CanDoLeftAction()) return;
                _leftInAnimation = true;
                _leftCurrentAmmo--;
            }
            else {
                if (!CanDoRightAction()) return;
                _rightInAnimation = true;
                _rightCurrentAmmo--;
            }
            
            IEnumerator SetLeftNotInAnimation() {
                yield return new WaitForSeconds(HalfCrotchet);
                _leftInAnimation = false;
            }

            IEnumerator SetRightNotInAnimation() {
                yield return new WaitForSeconds(HalfCrotchet);
                _rightInAnimation = false;
            }
            
            ShootForward(damage);
            _animator.CrossFade(animationName, _crossFade, -1, 0f);
            StartCoroutine(left ? SetLeftNotInAnimation() : SetRightNotInAnimation());
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

        public override bool CanDoLeftAction() => !_leftInAnimation && _leftCurrentAmmo > 0;

        public override bool CanDoRightAction() => !_rightInAnimation && _rightCurrentAmmo > 0;

        public override bool CanDoBothAction() => false;
        public override void StartReload() {
            _leftInAnimation = true;
            _rightInAnimation = true;
            Conductor.Instance.DisableNextInteractions(1);
            
            IEnumerator AfterAnimation() {
                yield return new WaitForSeconds(Crotchet + Conductor.Instance.HalfBeatHitWindow);
                CanFastReload = true;
                yield return new WaitForSeconds(Crotchet);
                // Did not hit next beat -> force slow reload
                if (IsReloading && CanFastReload) SlowReload();
            }
            
            Conductor.Instance.AppendOnNextBeat(() => {
                _animator.CrossFade("Reload Start", _crossFade, -1, 0f);
                IsReloading = true;
                
                StartCoroutine(AfterAnimation());
            });
        }
        public override void FastReload() {
            CanFastReload = false;
            _animator.CrossFade("Reload Fast", _crossFade, -1, 0f);
            IEnumerator SetNotInAnimation() {
                yield return new WaitForSeconds(HalfCrotchet);
                _leftInAnimation = false;
                _rightInAnimation = false;
                
                _leftCurrentAmmo = _maxAmmo;
                _rightCurrentAmmo = _maxAmmo;

                IsReloading = false;
            }

            if (_reloadRemoveInAnimation != null) StopCoroutine(_reloadRemoveInAnimation);
            _reloadRemoveInAnimation = StartCoroutine(SetNotInAnimation());
        }
        public override void SlowReload() {
            CanFastReload = false;
            _animator.CrossFade("Reload Slow", _crossFade, -1, 0f);
            Conductor.Instance.DisableNextInteractions(1);
            IEnumerator SetNotInAnimation() {
                yield return new WaitForSeconds(2 * Crotchet);
                _leftInAnimation = false;
                _rightInAnimation = false;
                
                _leftCurrentAmmo = _maxAmmo;
                _rightCurrentAmmo = _maxAmmo;

                IsReloading = false;
            }

            StartCoroutine(SetNotInAnimation());
        }

        public override void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");

            HalfCrotchet = Conductor.Instance.SongData.HalfCrotchet;
            Crotchet = Conductor.Instance.SongData.Crotchet;
            
            _rayForward = Camera.main!.ScreenPointToRay;
            _leftCurrentAmmo = _maxAmmo;
            _rightCurrentAmmo = _maxAmmo;
            base.CalculateAnimationsSpeed();
            
            _leftInAnimation = true;
            _rightInAnimation = true;
            _animator.CrossFade("Selected", _crossFade, -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
                _rightInAnimation = false;
            }
            StartCoroutine(SetNotInAnimation(0.6f));

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

        private void AnimateWalk() {
            if (!IsWalking) return;
            
            if (_walkAnimationSwitch) {
                if (!_leftInAnimation) 
                    _animator.CrossFade("Walk Left", _crossFade, -1, 0f);
                _walkAnimationSwitch = false;
            }
            else {
                if (!_rightInAnimation) 
                    _animator.CrossFade("Walk Right", _crossFade, -1, 0f);
                _walkAnimationSwitch = true;
            }
        }

        public override void OnWeaponDeselected() => _unsubscribeFromEvents();
    }
}