using Core.StateMachine.Base;
using Interactable;
using UnityEngine;

namespace Enemy.Base {
    public abstract class EnemyBase : MonoBehaviour, IDamageable {
        protected StateMachine EnemyStateMachine;
        
        [SerializeField] protected int _maxHealth;
        protected int CurrentHealth;

        public abstract void TakeDamage(int value);
        public abstract void Die();
    }
}