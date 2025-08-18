using Core.Behaviour.FiniteStateMachine;
using Interactable;
using Interactable.Damageable;
using UnityEngine;

namespace Enemy.Base {
    public abstract class EnemyBase : DamageableBehaviour {
        protected StateMachine EnemyStateMachine;
        protected bool IsTarget;
        [SerializeField] private Vector3 _colliderSize;

        public Vector3 ColliderSize => _colliderSize;
        public void SetIsTarget() => IsTarget = true;
    }
}