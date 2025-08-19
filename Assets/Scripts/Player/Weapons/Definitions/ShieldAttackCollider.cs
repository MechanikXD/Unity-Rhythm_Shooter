using System.Collections.Generic;
using Core.Game;
using Interactable;
using Interactable.Damageable;
using UnityEngine;

namespace Player.Weapons.Definitions {
    [RequireComponent(typeof(Collider))]
    public class ShieldAttackCollider : MonoBehaviour {
        [SerializeField] private Collider _attackCollider;
        private int _damage;
        private readonly HashSet<IDamageable> _damagedThisAttack = new HashSet<IDamageable>();

        public void ActivateCollider(int damageValue) {
            _damage = damageValue;
            _attackCollider.enabled = true;
        }

        public void DeactivateCollider() {
            _damagedThisAttack.Clear();
            _attackCollider.enabled = false;
        }

        public void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent<IDamageable>(out var damageable) && 
                !_damagedThisAttack.Contains(damageable)) {
                IDamageable playerDamageable = GameManager.Instance.Player;
                damageable.TakeDamage(new DamageInfo(playerDamageable, damageable, _damage,
                    _attackCollider.bounds.center, other.contacts[0].point));
                _damagedThisAttack.Add(damageable);
            }
        }
    }
}