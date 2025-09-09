using System;
using System.Collections;
using Core.Behaviour.BehaviourInjection;
using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
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

        [SerializeField] private ParticleSystem _leftMuzzleFlash;
        [SerializeField] private ParticleSystem _rightMuzzleFlash;

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
                if (!CanDoLeftAction()) {
                    if (_leftCurrentAmmo <= 0) PlaySound(_emptyShot);
                    
                    return;
                }
                _leftInAnimation = true;
                _leftCurrentAmmo--;
                
                _leftActionBehaviour.Perform(damage);
                if (!_leftMuzzleFlash.gameObject.activeInHierarchy) {
                    _leftMuzzleFlash.gameObject.SetActive(true);
                }
                _leftMuzzleFlash.Play();
                
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
                if (!_rightMuzzleFlash.gameObject.activeInHierarchy) {
                    _rightMuzzleFlash.gameObject.SetActive(true);
                }
                _rightMuzzleFlash.Play();
                
                IEnumerator SetRightNotInAnimation() {
                    yield return new WaitForSeconds(HalfCrotchet);
                    _rightInAnimation = false;
                }
                
                _animator.CrossFade("Shoot Right", _crossFade, -1, 0f);
                StartCoroutine(SetRightNotInAnimation());
            }
        }
        
        private void ShootForward(int damage) {
            PlaySound(_shotSounds);
            var ray = ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out var hit, _maxShootDistance, IgnorePlayer) &&
                hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {

                var info = DamageInfoBuilder.PlayerAttack(damageable, hit.point);
                
                damageable.TakeDamage(info);
                PlayerEvents.OnDamageDealt(info);
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
            PlaySound(_reloadStartSound, ReloadStartPitch);
            Conductor.Instance.DisableNextInteractions(1);
            var sequenceBuilder = new ActionSequenceBuilder();
            sequenceBuilder.Append(Trigger.AfterBeat, _ => CanFastReload = true);
            sequenceBuilder.Append(Trigger.AfterBeat, () => IsReloading && CanFastReload,
                _ => SlowReload());
            var sequence = sequenceBuilder.ToSequence();
            var animationTimer = new CustomBeatTimer(sequence.Start, 1, 1);
            
            Conductor.Instance.AddOnNextBeat(() => {
                _animator.CrossFade("Reload Start", _crossFade, -1, 0f);
                IsReloading = true;
                
                animationTimer.StartTimer();
            });
        }
        public override void FastReload() {
            CanFastReload = false;
            PlaySound(_reloadFastSound, ReloadFastPitch);
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
            PlaySound(_reloadSlowSound, ReloadSlowPitch);
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
            Conductor.NextBeat += AnimateWalk;

            _unsubscribeFromEvents = () => {
                Conductor.NextBeat -= AnimateWalk;
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