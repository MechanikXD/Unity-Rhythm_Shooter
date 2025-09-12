using Core.Behaviour.FiniteStateMachine;
using Core.Game.VisualFX;
using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonTank.States;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonTank {
    public class SkeletonTank : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private EnemyAttackCollider _attackCollider;
        [SerializeField] private Transform _axePosition;
        [SerializeField] private TrailRenderer _trail;

        public TrailRenderer Trail => _trail;
        public Vector3 AxePosition => _axePosition.position;
        public EnemyAttackCollider AttackCollider => _attackCollider;

        [Header("Behaviour:")]
        [SerializeField] private float _normalIdleTime = 1.5f;
        [SerializeField] private float _shieldingTime = 3f;
        
        public float NormalIdleTime => _normalIdleTime;
        public float ShieldingTime => _shieldingTime;

        [Header("Animations:")]
        [SerializeField] private string _idleAnimationKey = "Paladin Idle";
        [SerializeField] private string _walkAnimationKey = "Paladin Walk";
        [SerializeField] private AnimationClip _deathAnimation;

        [SerializeField] private AnimationClip _blockStart;
        [SerializeField] private string _blockLoopKey = "Block Loop";
        [SerializeField] private string _blockExitKey = "Block Exit";

        [SerializeField] private AnimationClip _attackWindUpAnimation;
        [SerializeField] private AnimationClip _attackAnimation;
        
        public string IdleAnimationKey => _idleAnimationKey;
        public string WalkAnimationKey => _walkAnimationKey;
        public AnimationClip BlockStart => _blockStart;
        public string BlockLoopKey => _blockLoopKey;
        public string BlockExitKey => _blockExitKey;
        public AnimationClip AttackWindUpAnimation => _attackWindUpAnimation;
        public AnimationClip AttackAnimation => _attackAnimation;

        [Header("Animator Param Keys:")]
        [SerializeField] private string _slamWindUpSpeedKey = "SlamWindUp";
        [SerializeField] private string _slamAttackSpeedKey = "SlamAttack";
        private int _slamWindUp;
        private int _slamAttack;

        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _swingSounds;
        
        public AudioClip[] SwingSounds => _swingSounds;
        
        protected override void Start()
        {
            base.Start();
            VFXManager.Instance.RegisterParticles(_attackTelegraph.Particle, 3);
        }

        protected override State[] InitializeStates() {
            var idleState = new Idle(this);
            var shieldIdle = new Shielding(this);
            var attackState = new Attack(this);
            var walkToPlayer = new ChasePlayer(this);
            
            return new State[] {
                idleState,
                shieldIdle,
                attackState,
                walkToPlayer
            };
        }

        private void UpdateAnimatorParams() {
            _slamWindUp = Animator.StringToHash(_slamWindUpSpeedKey);
            _slamAttack = Animator.StringToHash(_slamAttackSpeedKey);
        }

        protected override void UpdateAnimationSpeed() {
            UpdateAnimatorParams();
            var crotchet = Conductor.Instance.SongData.Crotchet;

            Animator.SetFloat(_slamWindUp, _attackWindUpAnimation.length / crotchet);
            Animator.SetFloat(_slamAttack, _attackAnimation.length / (2 * crotchet));
        }

        public override void Die() {
            base.Die();
            StateMachine.StopMachine();
            _animator.CrossFade(_deathAnimation.name, _crossFade, -1, 0f);

            Destroy(gameObject, _deathAnimation.length);
        }
    }
}