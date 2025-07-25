using Core.Behaviour.FiniteStateMachine;
using Interactable;
using UnityEngine;

namespace Enemy.Base {
    public abstract class EnemyBase : MonoBehaviour, IDamageable {
        protected StateMachine EnemyStateMachine;
        protected bool IsTarget;
        [SerializeField] private Vector3 _colliderSize;
        
        [SerializeField] protected int _maxHealth;
        protected int CurrentHealth;

        public Vector3 ColliderSize => _colliderSize;
        public void SetIsTarget() => IsTarget = true;

        public abstract void TakeDamage(int value);
        public abstract void Die();

    }
}