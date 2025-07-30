using System;
using Core.Music;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    public class AnyDirectionTestWeapon : WeaponBase {
        private const float MaxHitDistance = 50f;
        private Func<Vector3, Ray> _rayForward;
        
        public override void LeftPerfectAction() => ShootForward(2);

        public override void LeftGoodAction() => ShootForward(1);

        public override void RightPerfectAction() => ShootForward(2);

        public override void RightGoodAction() => ShootForward(1);

        public override void OnReload() {
            throw new NotImplementedException();
        }
        // TODO: Do all of the reload with animation triggers
        protected override void StartReload() {
            var relative = Conductor.Instance.GetRelativeType();
            if (relative == BeatHitRelative.Early) {
                Conductor.Instance.DisableNextInteractions(2);
            }
            else Conductor.Instance.DisableNextInteractions(1, halfBeats:1);
        }

        protected override void StartSlowReload() {
            Conductor.Instance.DisableNextInteractions(4);
            // Reload
        }

        protected override void ContinueWithSlowReload() {
            Conductor.Instance.DisableNextInteractions(1, halfBeats:1);
            // Reload 
        }

        protected override void FastReload() {
            InReload = false;
            // Reload
        }

        public override void OnWeaponSelected() {
            base.OnWeaponSelected();
            _rayForward = Camera.main!.ScreenPointToRay;
        }

        private void ShootForward(int damage) {
            if (Physics.Raycast(_rayForward(new Vector2(Screen.width / 2f, Screen.height / 2f)),
                    out var hit, MaxHitDistance) &&
                hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {
                PlayerEvents.OnDamageCalculated(ref damage);
                
                damageable.TakeDamage(damage);
                PlayerEvents.OnDamageDealt();
            }
            else {
                PlayerEvents.OnAttackFailed();
            }
        }
    }
}