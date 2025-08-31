using Interactable.Damageable;
using UnityEngine;

namespace Interactable.AttackCollider {
    [RequireComponent(typeof(Collider))]
    public abstract class AttackHitBox : MonoBehaviour {
        [SerializeField] private Collider _attackCollider;
        [SerializeField] private DamageableBehaviour _owner;

        public virtual void ActivateCollider() {
            _attackCollider.enabled = true;
        }

        public virtual void DeactivateCollider() {
            _attackCollider.enabled = false;
        }

        protected abstract void ProcessAttack(Collision other);

        public void OnCollisionEnter(Collision other) => ProcessAttack(other);
    }
}