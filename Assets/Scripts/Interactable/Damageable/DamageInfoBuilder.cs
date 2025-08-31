using Core.Game;
using Enemy.Base;
using UnityEngine;

namespace Interactable.Damageable {
    public static class DamageInfoBuilder {
        public static DamageInfo PlayerAttack(EnemyBase target) {
            var player = GameManager.Instance.Player;
            return new DamageInfo(player, target, player.CurrentDamage, player.Position,
                target.Position);
        }
        
        public static DamageInfo PlayerAttack(IDamageable target, Vector3 hitPosition) {
            var player = GameManager.Instance.Player;
            return new DamageInfo(player, target, player.CurrentDamage, player.Position,
                hitPosition);
        }

        public static DamageInfo SourceLess(IDamageable target, int damageValue, Vector3 hitPosition) {
            return new DamageInfo(null, target, damageValue, Vector3.zero, hitPosition);
        }

        public static DamageInfo SourceLess(DamageableBehaviour target, int damageValue) {
            return new DamageInfo(null, target, damageValue, Vector3.zero, target.Position);
        }

        public static DamageInfo EnemyOnPlayer(EnemyBase enemy) {
            var player = GameManager.Instance.Player;
            return new DamageInfo(enemy, player, enemy.CurrentDamage, enemy.Position,
                player.Position);
        }

        public static DamageInfo EnemyOnPlayer(DamageableBehaviour enemy) {
            var player = GameManager.Instance.Player;
            return new DamageInfo(enemy, player, enemy.CurrentDamage, enemy.Position,
                player.Position);
        }
    }
}