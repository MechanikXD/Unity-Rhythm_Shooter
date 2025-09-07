using System.Collections.Generic;
using Interactable.Damageable;
using UnityEngine;

namespace Player.Weapons.Definitions {
    [RequireComponent(typeof(Collider))]
    public class ShieldAttackCollider : MonoBehaviour {
        [SerializeField] private Collider _attackCollider;
        private readonly HashSet<IDamageable> _damagedThisAttack = new HashSet<IDamageable>();

        public void ActivateCollider() {
            _attackCollider.enabled = true;
        }

        public void DeactivateCollider() {
            _damagedThisAttack.Clear();
            _attackCollider.enabled = false;
        }

        public void OnCollisionEnter(Collision other) {
            if (!other.gameObject.TryGetComponent<IDamageable>(out var damageable) ||
                _damagedThisAttack.Contains(damageable)) return;

            var info = DamageInfoBuilder.PlayerAttack(damageable, other.contacts[0].point);
            damageable.TakeDamage(info);
            _damagedThisAttack.Add(damageable);
        }
    }
}