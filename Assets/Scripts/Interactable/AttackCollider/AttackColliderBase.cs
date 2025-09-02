using Interactable.Damageable;
using UnityEngine;

namespace Interactable.AttackCollider {
    [RequireComponent(typeof(Collider))]
    public abstract class AttackColliderBase : MonoBehaviour {
        [SerializeField] protected Collider _attackCollider;
        [SerializeField] protected DamageableBehaviour _owner;

        public void SetOwner(DamageableBehaviour owner) => _owner = owner;

        public virtual void Enable() => _attackCollider.enabled = true;

        public abstract void Reset();

        public virtual void Disable() => _attackCollider.enabled = false;

        protected abstract void ProcessAttack(Collision other);

        public void OnCollisionEnter(Collision other) => ProcessAttack(other);
    }
}