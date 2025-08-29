using Enemy.States.Base;
using Interactable.Damageable;
using UnityEngine;
using UnityEngine.AI;
using StateMachine = Core.Behaviour.FiniteStateMachine.StateMachine;

namespace Enemy.Base {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : DamageableBehaviour {
        public const float Proximity = 1f;
        protected StateMachine EnemyStateMachine;
        public NavMeshAgent Agent { private set; get; }
        protected bool IsTarget;
        [SerializeField] private Vector3 _colliderSize;

        public Vector3 ColliderSize => _colliderSize;
        public void SetIsTarget() => IsTarget = true;

        public abstract bool HasState<T>() where T : EnemyState;

        protected virtual void Update() => EnemyStateMachine.CurrentState.FrameUpdate();

        protected virtual void FixedUpdate() => EnemyStateMachine.CurrentState.FixedUpdate();

        protected override void Awake() {
            base.Awake();
            EnemyStateMachine = new StateMachine();
            Agent = GetComponent<NavMeshAgent>();
        }
    }
}