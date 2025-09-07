using Core.Behaviour.FiniteStateMachine;
using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonArcher.States;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher {
    public class SkeletonArcher : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private Transform _arrowSpawnPoint;
        [SerializeField] private EnemyArrow _enemyAttack;
        [SerializeField] private Transform _bowPosition;

        public Vector3 BowPosition => _bowPosition.position;
        public Transform ArrowSpawnPoint => _arrowSpawnPoint;
        public EnemyArrow ArrowPrefab => _enemyAttack;

        [Header("Behaviour:")]
        [SerializeField] private float _idleTime = 1f;
        [SerializeField] private float _arrowHeightCorrection = 0.2f;
        [SerializeField] private Vector2 _repositionBounds = new Vector2(7f, 20f);

        public float IdleTime => _idleTime;
        public float ArrowHeightCorrection => _arrowHeightCorrection;
        public Vector2 RepositionBounds => _repositionBounds;
        
        [Header("Animations:")]
        [SerializeField] private string _idleAnimationKey = "Archer Idle";
        [SerializeField] private string _runAnimationKey = "Archer Run";
        [SerializeField] private string _deathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        
        public string IdleAnimationKey => _idleAnimationKey;
        public string RunAnimationKey => _runAnimationKey;
        public string DeathAnimationKey => _deathAnimationKey;
        public AnimationClip AttackStateEnter => _attackStateEnter;
        public AnimationClip AttackStateLoop => _attackStateLoop;
        public AnimationClip AttackStateExit => _attackStateExit;
        
        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _arrowLoadSounds;
        [SerializeField] private AudioClip[] _arrowReleaseSounds;

        public AudioClip[] ArrowLoadSounds => _arrowLoadSounds;
        public AudioClip[] ArrowReleaseSounds => _arrowReleaseSounds;

        [Header("Animator Param Keys:")]
        [SerializeField] private string _attackStartSpeedKey = "AttackStartSpeed";

        private int _attackStartSpeed;

        protected override State[] InitializeStates() {
            var idleState = new Idle(this);
            var repositionState = new Reposition(this);
            var attackState = new Attack(this);

            return new State[] {
                idleState,
                repositionState,
                attackState
            };
        }

        protected override void UpdateAnimationSpeed() {
            _attackStartSpeed = Animator.StringToHash(_attackStartSpeedKey);
            
            _animator.SetFloat(_attackStartSpeed, 
                _attackStateEnter.length / Conductor.Instance.SongData.Crotchet);
        }
        
        public override void Die() {
            base.Die();
            _animator.CrossFade(_deathAnimationKey, _crossFade, -1, 0f);
            
            Destroy(gameObject, _deathAnimationKey.Length);
        }
    }
}