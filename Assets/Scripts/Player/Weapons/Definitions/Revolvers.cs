using System;
using System.Collections;
using Core.Behaviour.BehaviourInjection;
using Core.Game;
using Core.Music;
using Interactable.Damageable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons.Definitions {
    public class Revolvers : WeaponBase {
        private bool _walkAnimationSwitch;
        
        private int _leftCurrentAmmo;
        private int _rightCurrentAmmo;

        private bool _leftInAnimation;
        private bool _rightInAnimation;

        private BehaviourInjection<int> _leftActionBehaviour;
        private BehaviourInjection<int> _rightActionBehaviour;

        private Coroutine _reloadRemoveInAnimation;
        private Action _unsubscribeFromEvents;

        public override void LeftPerfectAction() => 
            PerformAction(4, true);
        public override void LeftGoodAction() => 
            PerformAction(3, true);
        public override void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, true);
        }

        public override void RightPerfectAction() => 
            PerformAction(4, false);
        public override void RightGoodAction() => 
            PerformAction(3, false);
        public override void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(1);
            PerformAction(1, false);
        }
        
        private void PerformAction(int damage, bool left) {
            if (left) {
                if (!CanDoLeftAction()) return;
                _leftInAnimation = true;
                _leftCurrentAmmo--;
                
                _leftActionBehaviour.Perform(damage);
                
                IEnumerator SetLeftNotInAnimation() {
                    yield return new WaitForSeconds(HalfCrotchet);
                    _leftInAnimation = false;
                }
                
                _animator.CrossFade("Shoot Left", _crossFade, -1, 0f);
                StartCoroutine(SetLeftNotInAnimation());
            }
            else {
                if (!CanDoRightAction()) return;
                _rightInAnimation = true;
                _rightCurrentAmmo--;
                
                _rightActionBehaviour.Perform(damage);
                
                IEnumerator SetRightNotInAnimation() {
                    yield return new WaitForSeconds(HalfCrotchet);
                    _rightInAnimation = false;
                }
                
                _animator.CrossFade("Shoot Right", _crossFade, -1, 0f);
                StartCoroutine(SetRightNotInAnimation());
            }
        }
        
        private void ShootForward(int damage) {
            var ray = ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out var hit, _maxShootDistance) &&
                hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {
                
                var player = GameManager.Instance.Player;
                var calculatedDamage = player.GetCalculatedDamage(damage);
                var damageInfo = new DamageInfo(player, damageable, calculatedDamage, ray.origin,
                    hit.point);
                
                damageable.TakeDamage(damageInfo);
                PlayerEvents.OnDamageDealt(damageInfo);
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
            
            Conductor.Instance.AddOnNextBeat(() => {
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
            base.OnWeaponSelected();
            _leftCurrentAmmo = _maxAmmo;
            _rightCurrentAmmo = _maxAmmo;

            _leftActionBehaviour = new BehaviourInjection<int>(ShootForward);
            _rightActionBehaviour = new BehaviourInjection<int>(ShootForward);
            
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
            Conductor.Instance.AddRepeatingAction("Weapon Walk", AnimateWalk);

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