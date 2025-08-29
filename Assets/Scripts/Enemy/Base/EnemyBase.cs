using Enemy.States.Base;
using Interactable.Damageable;
using UnityEngine;
using UnityEngine.AI;
using StateMachine = Core.Behaviour.FiniteStateMachine.StateMachine;

namespace Enemy.Base {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : DamageableBehaviour {
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _crossFade;
        public float CrossFade => _crossFade;
        public Animator Animator => _animator;
        public const float Proximity = 1f;
        protected StateMachine EnemyStateMachine;
        public NavMeshAgent Agent { private set; get; }
        protected bool IsTarget;
        [SerializeField] private Vector3 _colliderSize;
        public Vector3 Forward => transform.forward;

        public Vector3 ColliderSize => _colliderSize;
        public void SetIsTarget() => IsTarget = true;

        public abstract bool HasState<T>() where T : EnemyState;

        protected virtual void Update() => EnemyStateMachine.CurrentState.FrameUpdate();

        protected virtual void FixedUpdate() => EnemyStateMachine.CurrentState.FixedUpdate();

        public void PlayAnimation(string animationName) {
            Animator.CrossFade(animationName, CrossFade, -1, 0f);
        }

        protected override void Awake() {
            base.Awake();
            EnemyStateMachine = new StateMachine();
            Agent = GetComponent<NavMeshAgent>();
        }
    }
}