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

        [SerializeField] private int _maxAmmoPerRevolver = 6;
        private int _leftCurrentAmmo;
        private int _rightCurrentAmmo;

        private bool _leftInAnimation;
        private bool _rightInAnimation;
        private bool _isWalking;
        private Action _unsubscribeFromEvents;

        private Coroutine _reloadRemoveInAnimation;
        
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
                if (!CanDoLeftAction()) return;
                _leftInAnimation = true;
                _leftCurrentAmmo--;
            }
            else {
                if (!CanDoRightAction()) return;
                _rightInAnimation = true;
                _rightCurrentAmmo--;
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
            StartCoroutine(left ? SetLeftNotInAnimation(animationTime) : SetRightNotInAnimation(animationTime));
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
            
            IEnumerator AfterAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                CanFastReload = true;
                yield return new WaitForSeconds(Conductor.Instance.SongData.Crotchet);
                // Did not hit next beat -> force slow reload
                if (IsReloading && CanFastReload) SlowReload();
            }
            
            Conductor.Instance.AppendOnNextBeat(() => {
                _animator.Play("Reload Start", -1, 0f);
                IsReloading = true;
                
                StartCoroutine(AfterAnimation(Conductor.Instance.SongData.Crotchet +
                                              Conductor.Instance.HalfBeatHitWindow));
            });
        }
        public override void FastReload() {
            CanFastReload = false;
            _animator.Play("Reload Fast", -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
                _rightInAnimation = false;
                
                _leftCurrentAmmo = _maxAmmoPerRevolver;
                _rightCurrentAmmo = _maxAmmoPerRevolver;

                IsReloading = false;
            }

            if (_reloadRemoveInAnimation != null) StopCoroutine(_reloadRemoveInAnimation);
            _reloadRemoveInAnimation = StartCoroutine(SetNotInAnimation(37f / 60f));
        }
        public override void SlowReload() {
            CanFastReload = false;
            _animator.Play("Reload Slow", -1, 0f);
            Conductor.Instance.DisableNextInteractions(1);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
                _rightInAnimation = false;
                
                _leftCurrentAmmo = _maxAmmoPerRevolver;
                _rightCurrentAmmo = _maxAmmoPerRevolver;

                IsReloading = false;
            }

            StartCoroutine(SetNotInAnimation(80f / 60f));
        }

        public override void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");
            
            _rayForward = Camera.main!.ScreenPointToRay;
            _leftCurrentAmmo = _maxAmmoPerRevolver;
            _rightCurrentAmmo = _maxAmmoPerRevolver;
            
            _leftInAnimation = true;
            _rightInAnimation = true;
            _animator.Play("Selected", -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _leftInAnimation = false;
                _rightInAnimation = false;
            }
            StartCoroutine(SetNotInAnimation(0.6f));

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