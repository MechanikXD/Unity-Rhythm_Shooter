using System;
using System.Collections;
using Core.Game;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    [Serializable]
    public class Revolvers : WeaponBase {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _maxHitDistance = 50f;
        [SerializeField] private float _intervalBetweenWalk = 0.35f;
        private float _currentWalkInterval;
        private bool _walkAnimationSwitch;
        private Func<Vector3, Ray> _rayForward;

        private int _maxAmmoPerRevolver = 6;
        private int _leftCurrentAmmo;
        private int _rightCurrentAmmo;

        private bool _leftInAnimation;
        private bool _rightInAnimation;
        private bool _isWalking;
        private Action _unsubscribeFromEvents;
        
        public override void LeftPerfectAction() {
            if (_leftInAnimation) return;
            
            ShootForward(2);
            // TODO: Does not play animation more than once in a row
            _animator.Play("Shoot Left");
            _leftInAnimation = true;
            GameManager.Instance.StartCoroutine(SetLeftNotInAnimation(1f / 3f));
        }

        public override void LeftGoodAction() {
            if (_leftInAnimation) return;
            
            ShootForward(1);
            _animator.Play("Shoot Left");
            _leftInAnimation = true;
            GameManager.Instance.StartCoroutine(SetLeftNotInAnimation(1f / 3f));
        }

        public override void RightPerfectAction() {
            if (_rightInAnimation) return;
            
            ShootForward(2);
            _animator.Play("Shoot Right");
            _rightInAnimation = true;
            GameManager.Instance.StartCoroutine(SetRightNotInAnimation(1f / 3f));
        }

        public override void RightGoodAction() {
            if (_rightInAnimation) return;
            
            ShootForward(1);
            _animator.Play("Shoot Right");
            _rightInAnimation = true;
            GameManager.Instance.StartCoroutine(SetRightNotInAnimation(1f / 3f));
        }
        
        private void ShootForward(int damage) {
            if (Physics.Raycast(_rayForward(new Vector2(Screen.width / 2f, Screen.height / 2f)),
                    out var hit, _maxHitDistance) &&
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

        public override bool CanDoBothAction() => !_leftInAnimation || !_rightInAnimation;

        public override void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");
            
            _rayForward = Camera.main!.ScreenPointToRay;
            
            _leftInAnimation = true;
            _rightInAnimation = true;
            _animator.Play("Selected");
            GameManager.Instance.StartCoroutine(SetNotInAnimation(0.6f));

            void SetIsWalking() => _isWalking = true;
            void SetNotWalking() => _isWalking = false;

            PlayerEvents.StartWalking += SetIsWalking;
            PlayerEvents.StoppedWalking += SetNotWalking;

            _unsubscribeFromEvents = () => {
                PlayerEvents.StartWalking -= SetIsWalking;
                PlayerEvents.StoppedWalking -= SetNotWalking;
            };
        }

        public override void WeaponUpdate() {
            if (!_isWalking) return;
            
            if (_currentWalkInterval < _intervalBetweenWalk) {
                _currentWalkInterval += Time.deltaTime;
            }
            else {
                _currentWalkInterval = 0f;
                if (_walkAnimationSwitch) {
                    if (!_leftInAnimation) _animator.Play("Walk Left");
                    _walkAnimationSwitch = false;
                }
                else {
                    if (!_rightInAnimation) _animator.Play("Walk Right");
                    _walkAnimationSwitch = true;
                }
            }
        }

        public override void OnWeaponDeselected() => _unsubscribeFromEvents();

        private IEnumerator SetLeftNotInAnimation(float delay) {
            yield return new WaitForSeconds(delay);
            _leftInAnimation = false;
        }

        private IEnumerator SetRightNotInAnimation(float delay) {
            yield return new WaitForSeconds(delay);
            _rightInAnimation = false;
        }

        private IEnumerator SetNotInAnimation(float delay) {
            yield return new WaitForSeconds(delay);
            _leftInAnimation = false;
            _rightInAnimation = false;
        }
    }
}