using Interactable.Damageable;
using Player;
using UnityEngine;

namespace Interactable.AttackCollider {
    public class EnemyAttackCollider : AttackColliderBase {
        private bool _hasDamagedPlayer;

        public override void Enable() {
            base.Enable();
            _hasDamagedPlayer = false;
        }

        public override void Reset() => _hasDamagedPlayer = false;

        protected override void ProcessAttack(Collision other) {
            if (_hasDamagedPlayer ||
                !other.gameObject.TryGetComponent<PlayerController>(out var player)) {
                return;
            }

            var info = DamageInfoBuilder.EnemyOnPlayer(_owner);
            player.TakeDamage(info);
        }
    }
}