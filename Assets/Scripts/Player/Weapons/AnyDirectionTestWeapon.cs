using System;
using Interactable;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    public class AnyDirectionTestWeapon : WeaponBase {
        private const float MaxHitDistance = 50f;
        private Func<Vector3, Ray> _rayForward;

        public override void LeftPerfectAction() => ShootForward(3);

        public override void LeftGoodAction() => ShootForward(2);

        public override void RightPerfectAction() => ShootForward(3);

        public override void RightGoodAction() => ShootForward(2);

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