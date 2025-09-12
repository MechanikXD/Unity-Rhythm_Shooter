using System;
using System.Collections;
using System.Collections.Generic;
using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Core.Game.VisualFX;
using Enemy.Tools;
using Interactable.Damageable;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using StateMachine = Core.Behaviour.FiniteStateMachine.StateMachine;

namespace Enemy.Base {
    [RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider))]
    public abstract class EnemyBase : DamageableBehaviour {
        [Header("Enemy Params: ")]
        // ---------- General ----------
        [SerializeField] private LayerMask _obstacleLayerMask;
        private BoxCollider _collider;
        
        protected bool IsTarget;
        public Vector3 Forward => transform.forward;
        public Vector3 ColliderSize => _collider.bounds.size;
        
        // ---------- State Machine ----------
        private StateMachine _enemyStateMachine;
        public StateMachine StateMachine => _enemyStateMachine;
        private Dictionary<Type, State> _enemyStates;
        public Dictionary<Type, State> States => _enemyStates;
        
        // ---------- NavMesh ----------
        public NavMeshAgent Agent { get; private set; }
        [SerializeField] private AgentRotationController _rotationController;
        public AgentRotationController Rotation => _rotationController;
        public const float PlayerProximity = 2.5f;  // Distance to player when considered "close"
        
        // ---------- Animator ----------
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _crossFade;
        protected Animator Animator => _animator;
        
        [SerializeField] protected AttackTelegraph _attackTelegraph;
        public AttackTelegraph AttackTelegraph => _attackTelegraph;
        
        // ---------- Player Cache ----------
        public Transform PlayerTransform { get; private set; }
        public Vector3 DirectionToPlayer =>
            (PlayerTransform.position - transform.position).normalized;
        public float DistanceToPlayer => 
            Vector3.Distance(PlayerTransform.position, transform.position);
        
        // ---------- Audio ----------
        [SerializeField] protected Vector2 _walkSoundDelay;
        [SerializeField] protected float _soundDistance;

        public Vector2 WalkSoundDelay => _walkSoundDelay;
        
        public static event Action<EnemyDefeatedInfo> EnemyDefeated;
        public static event Action<EnemyDefeatedInfo> TargetDefeated;
        public static event Action<EnemyDefeatedInfo> NormalDefeated;
        
        public void SetIsTarget() => IsTarget = true;

        protected virtual void Update() => _enemyStateMachine.CurrentState.FrameUpdate();

        protected virtual void FixedUpdate() => _enemyStateMachine.CurrentState.FixedUpdate();

        public void PlayAnimation(string animationName) => 
            Animator.CrossFade(animationName, _crossFade, -1, 0f);
        public void PlayAnimation(AnimationClip anim) => 
            Animator.CrossFade(anim.name, _crossFade, -1, 0f);

        protected override void Awake() {
            base.Awake();
            Initialize();
        }

        protected virtual void Start() {
            UpdatePlayerReference();
            UpdateAnimationSpeed();
            InitializeStateMachine();
        }

        protected abstract void UpdateAnimationSpeed();

        public override void Die() {
            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyDefeated?.Invoke(info);
            if (IsTarget) TargetDefeated?.Invoke(info);
            else NormalDefeated?.Invoke(info);
        }
        
        public override void TakeDamage(DamageInfo damageInfo) {
            base.TakeDamage(damageInfo);
            PlayRandomSound(_hurtSounds);
        }
        
        public void PlayWalkSound() => PlayRandomSound(_stepSounds);

        public void PlayAttackTelegraphParticle(Vector3 position) {
            var instance = Instantiate(_attackTelegraph, transform);
            instance.transform.position = position;
            instance.Play();
        }

        #region AI / Navigation
        
        private void UpdatePlayerReference() => 
            PlayerTransform = GameManager.Instance.Player.transform;

        public bool NearPoint(Vector3 point, float proximity) =>
            Vector3.Distance(Position, point) < proximity;
        
        public bool HasLineOfSightWithPlayer() {
            var playerPosition = PlayerTransform.position;
            var distance = Vector3.Distance(Position, playerPosition);
    
            var eyeLevel = Position + Vector3.up * 1.5f;
            var targetEyeLevel = playerPosition + Vector3.up * 1.5f;
            var direction = (targetEyeLevel - eyeLevel).normalized;
    
            return !Physics.Raycast(eyeLevel, direction, distance, _obstacleLayerMask);
        }
        
        public bool HasLineOfSightWithPlayer(Vector3 position) {
            var playerPosition = PlayerTransform.position;
            var distance = Vector3.Distance(position, playerPosition);
    
            var eyeLevel = position + Vector3.up * 1.5f;
            var targetEyeLevel = playerPosition + Vector3.up * 1.5f;
            var direction = (targetEyeLevel - eyeLevel).normalized;
    
            return !Physics.Raycast(eyeLevel, direction, distance, _obstacleLayerMask);
        }

        public bool IsNearPlayer(float proximity) {
            return Vector3.Distance(Position, PlayerTransform.position) < proximity;
        }
        
        #endregion
        
        #region Initialization Methods
        
        private void Initialize() {
            Agent = GetComponent<NavMeshAgent>();
            _collider = GetComponent<BoxCollider>();
        }
        
        private void InitializeStateMachine() {
            _enemyStateMachine = new StateMachine();
            var states = InitializeStates();
            LoadEnemyStates(states);
            _enemyStateMachine.Initialize(states[0]);
        }
        /// <summary>
        /// A workaround to prevent NullRef by forcing EnemyBase to fully initialize state machine.
        /// NOTE: first state in the array must be starting state!
        /// </summary>
        /// <returns> Array of all possible enemy states, where first is initial state </returns>
        protected abstract State[] InitializeStates();

        private void LoadEnemyStates(IEnumerable<State> states) {
            _enemyStates = new Dictionary<Type, State>();

            foreach (var state in states) {
                if (!_enemyStates.TryAdd(state.GetType(), state)) {
                    Debug.LogWarning(
                        $"Enemy {gameObject.name} have 2 or more states with the same type. Latest states will not be added!");
                }
            }
        }
        
        #endregion
    }
    
    public struct EnemyDefeatedInfo {
        public Type EnemyType { get; }
        public Vector3 Position { get; }
        public bool WasTarget { get; }
        public int ID { get; }
            
        public EnemyDefeatedInfo(Type type, int id, Vector3 position, bool wasTarget = false) {
            EnemyType = type;
            ID = id;
            Position = position;
            WasTarget = wasTarget;
        }
    }
}