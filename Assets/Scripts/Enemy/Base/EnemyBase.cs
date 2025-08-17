using Core.Behaviour.FiniteStateMachine;
using Interactable;
using UnityEngine;

namespace Enemy.Base {
    public abstract class EnemyBase : MonoBehaviour, IDamageable {
        protected StateMachine EnemyStateMachine;
        protected bool IsTarget;
        [SerializeField] private Vector3 _colliderSize;

        [SerializeField] protected int _maxHealth;

        protected int CurrentHealth { get; set; }

        int IDamageable.CurrentHealth {
            get => CurrentHealth;
            set => CurrentHealth = value;
        }

        public Vector3 ColliderSize => _colliderSize;
        public void SetIsTarget() => IsTarget = true;

        public int CurrentStatuses { get; }
        public abstract void TakeDamage(DamageInfo damageInfo);
        public abstract void Parried(DamageInfo damageInfo);

        public void ApplyStatus(int status) {
            throw new System.NotImplementedException();
        }

        public abstract void Die();
    }
}