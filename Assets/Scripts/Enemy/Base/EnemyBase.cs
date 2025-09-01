using Core.Game;
using Enemy.AgentRotation;
using Interactable.Damageable;
using Player;
using UnityEngine;
using UnityEngine.AI;
using StateMachine = Core.Behaviour.FiniteStateMachine.StateMachine;

namespace Enemy.Base {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : DamageableBehaviour {
        protected StateMachine EnemyStateMachine;
        private Transform _playerTransform;
        public Transform PlayerTransform => _playerTransform;
        
        public NavMeshAgent Agent { private set; get; }
        [SerializeField] private AgentRotationController _rotationController;
        public AgentRotationController Rotation => _rotationController;
        public const float PlayerProximity = 2.5f;
        public Vector3 Forward => transform.forward;
        
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _crossFade;
        public float CrossFade => _crossFade;
        public Animator Animator => _animator;
        
        [SerializeField] private Vector3 _colliderSize;
        public Vector3 ColliderSize => _colliderSize;
        protected bool IsTarget;
        public void SetIsTarget() => IsTarget = true;

        protected virtual void Update() => EnemyStateMachine.CurrentState.FrameUpdate();

        protected virtual void FixedUpdate() => EnemyStateMachine.CurrentState.FixedUpdate();

        public void PlayAnimation(string animationName) {
            Animator.CrossFade(animationName, CrossFade, -1, 0f);
        }

        protected override void Awake() {
            base.Awake();
            _playerTransform = GameManager.Instance.Player.transform;
            EnemyStateMachine = new StateMachine();
            Agent = GetComponent<NavMeshAgent>();
        }

        public bool AgentAtDestination() => Agent.pathStatus == NavMeshPathStatus.PathComplete;

        public bool NearPoint(Vector3 point, float proximity) =>
            Vector3.Distance(Position, point) < proximity;
        
        public bool HasLineOfSightWithPlayer() {
            var playerPosition = PlayerTransform.position;
            var distance = Vector3.Distance(Position, playerPosition);
            // RayCast from eye level
            var eyeLevel = Position + Vector3.up * 1.5f;
            var targetEyeLevel = playerPosition + Vector3.up * 1.5f;
            
            return Physics.Raycast(eyeLevel, (targetEyeLevel - eyeLevel).normalized, out var hit,
                distance) && hit.transform.gameObject.TryGetComponent<PlayerController>(out _);
        }
    }
}