using Core.Game;
using Enemy.AgentRotation;
using Interactable.Damageable;
using UnityEngine;
using UnityEngine.AI;
using StateMachine = Core.Behaviour.FiniteStateMachine.StateMachine;

namespace Enemy.Base {
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : DamageableBehaviour {
        protected StateMachine EnemyStateMachine;
        private Transform _playerTransform;
        public Transform PlayerTransform => _playerTransform;
        public Vector3 PlayerDirection =>
            (_playerTransform.position - transform.position).normalized;
        
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
        [SerializeField] private LayerMask _obstacleLayerMask;
        public void SetIsTarget() => IsTarget = true;

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

        public void UpdatePlayerReference() {
            _playerTransform = GameManager.Instance.Player.transform;
        }

        public bool AgentAtDestination() => Agent.pathStatus == NavMeshPathStatus.PathComplete;

        public bool NearPoint(Vector3 point, float proximity) =>
            Vector3.Distance(Position, point) < proximity;
        
        
        public bool HasLineOfSightWithPlayer() {
            var playerPosition = PlayerTransform.position;
            var distance = Vector3.Distance(Position, playerPosition);
    
            var eyeLevel = Position + Vector3.up * 1.5f;
            var targetEyeLevel = playerPosition + Vector3.up * 1.5f;
            var direction = (targetEyeLevel - eyeLevel).normalized;
    
            // Check if any obstacles are in the way
            return !Physics.Raycast(eyeLevel, direction, distance, _obstacleLayerMask);
        }
        
        public bool HasLineOfSightWithPlayer(Vector3 position) {
            var playerPosition = PlayerTransform.position;
            var distance = Vector3.Distance(position, playerPosition);
    
            var eyeLevel = position + Vector3.up * 1.5f;
            var targetEyeLevel = playerPosition + Vector3.up * 1.5f;
            var direction = (targetEyeLevel - eyeLevel).normalized;
    
            // Check if any obstacles are in the way
            return !Physics.Raycast(eyeLevel, direction, distance, _obstacleLayerMask);
        }
    }
}