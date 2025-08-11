using System;
using System.Collections;
using Core.Game;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Weapons {
    public class Shotgun : WeaponBase {
        private Func<Vector3, Ray> _screenPointToRay;
        [SerializeField] private int _pelletCount = 10;
        [SerializeField] protected float _spreadAngle;
        private int _currentAmmoCount;
        private bool _hasAmmoInChamber;
        
        private bool _inAnimation;
        private Action _unsubscribeFromEvents;
        private Coroutine _reloadRemoveInAnimation;
        
        public override void LeftPerfectAction() => Shoot(1, _pelletCount);

        public override void LeftGoodAction() => Shoot(1, _pelletCount - 3);

        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            Shoot(1, _pelletCount - 5);
        }

        public override void RightPerfectAction() => Pump(HalfCrotchet);

        public override void RightGoodAction() => Pump(HalfCrotchet);

        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            Pump(Crotchet);
        }

        private void Shoot(int damage, int pelletCount) {
            if (!CanDoLeftAction()) return;
            
            _currentAmmoCount--;
            _hasAmmoInChamber = false;
            _inAnimation = true;
            
            var widthDeviation = Screen.width / 2f * (1f - _spreadAngle / 90f);
            var heightDeviation = Screen.height / 2f * (1f - _spreadAngle / 90f);

            var counter = pelletCount;
            while (counter > 0) {
                var ray = _screenPointToRay(
                    new Vector2(
                        Random.Range(widthDeviation, Screen.width - widthDeviation),
                        Random.Range(heightDeviation, Screen.height - heightDeviation)));
                
                if (Physics.Raycast(ray, out var hit, _maxShootDistance)) {
                    if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {
                        IDamageable playerDamageable = GameManager.Instance.Player;
                        damageable.TakeDamage(new DamageInfo(playerDamageable, damageable, damage, ray.origin,
                            hit.point));
                        PlayerEvents.OnDamageDealt();
                    }
                    else {
                        PlayerEvents.OnAttackFailed();
                    }
                }
                
                counter--;
            }
            
            IEnumerator SetNotInAnimation() {
                yield return new WaitForSeconds(HalfCrotchet);
                _inAnimation = false;
            }
            
            _animator.CrossFade("Shoot", _crossFade, -1, 0f);
            StartCoroutine(SetNotInAnimation());
        }

        private void Pump(float duration) {
            if (!CanDoRightAction()) return;
            
            _inAnimation = true;
            
            IEnumerator SetNotInAnimation() {
                yield return new WaitForSeconds(duration);
                _inAnimation = false;
                if (_hasAmmoInChamber) _currentAmmoCount--;
                else _hasAmmoInChamber = true;
            }
            
            _animator.CrossFade("Pump", _crossFade, -1, 0f);
            StartCoroutine(SetNotInAnimation());
        }

        public override bool CanDoLeftAction() => _currentAmmoCount > 0 && _hasAmmoInChamber;

        public override bool CanDoRightAction() => _currentAmmoCount > 0;

        public override bool CanDoBothAction() => false;

        public override void StartReload() {
            _inAnimation = true;
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
                _inAnimation = false;
                
                _currentAmmoCount = _maxAmmo;
                _hasAmmoInChamber = true;

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
                _inAnimation = false;
                
                _currentAmmoCount = _maxAmmo;
                _hasAmmoInChamber = true;

                IsReloading = false;
            }

            StartCoroutine(SetNotInAnimation());
        }

        public override void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");
            _screenPointToRay = Camera.main!.ScreenPointToRay;

            HalfCrotchet = Conductor.Instance.SongData.HalfCrotchet;
            Crotchet = Conductor.Instance.SongData.Crotchet;
            
            _currentAmmoCount = _maxAmmo;
            _hasAmmoInChamber = true;
            base.CalculateAnimationsSpeed();
            
            _inAnimation = true;
            _animator.CrossFade("Selected", _crossFade, -1, 0f);
            IEnumerator SetNotInAnimation(float delay) {
                yield return new WaitForSeconds(delay);
                _inAnimation = false;
            }
            StartCoroutine(SetNotInAnimation(1f));
            
            void AnimateWalk() {
                if (IsWalking && !_inAnimation) 
                    _animator.CrossFade("Walk", _crossFade, -1, 0f);
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

        public override void OnWeaponDeselected() => _unsubscribeFromEvents();
    }
}