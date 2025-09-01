using Core.Music;
using Enemy.Base;
using Enemy.States.Base;
using Enemy.Types.SkeletonArcher.States;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher {
    public class SkeletonArcher : EnemyBase {
        private ArcherIdleState _idleState;
        private RepositionState _repositionState;
        [SerializeField] private Transform _arrowSpawnPoint;
        private AttackState _attackState;

        [SerializeField] private EnemyArrow _enemyAttack;
        
        private const string IdleAnimationKey = "Archer Idle";
        private const string RunAnimationKey = "Archer Run";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        private readonly static int AttackStartSpeed = Animator.StringToHash("AttackStartSpeed");

        protected override void Awake() {
            base.Awake();
            
            _idleState = new ArcherIdleState(EnemyStateMachine, this, null, 1, IdleAnimationKey);
            EnemyStateMachine.Initialize(_idleState);
        }

        private void Start() {
            UpdatePlayerReference();
            
            _repositionState = new RepositionState(EnemyStateMachine, this,
                new EnemyState[] { _idleState }, RunAnimationKey, 5f);

            _attackState =
                new AttackState(EnemyStateMachine, this, new EnemyState[] { _repositionState },
                    _arrowSpawnPoint, _enemyAttack, _attackStateEnter.name, _attackStateLoop.name, _attackStateExit);
            
            _idleState.SetOutStates(new EnemyState[] { _attackState, _repositionState });
            
            _animator.SetFloat(AttackStartSpeed, _attackStateEnter.length / Conductor.Instance.SongData.Crotchet);
        }

        protected override void EnterParriedState() { }

        protected override void UpdateMoveSpeedOnCharacter() {
            _repositionState.SetMoveSpeed(_moveSpeed);
        }

        public override void Die() {
            _animator.CrossFade(DeathAnimationKey, _crossFade, -1, 0f);
            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyEvents.OnEnemyDefeated(info);
            if (IsTarget) EnemyEvents.OnTargetDefeated(info);
            else EnemyEvents.OnNormalDefeated(info);
            
            Destroy(gameObject, DeathAnimationKey.Length);
        }
    }
}