using Enemy.Base;
using Enemy.States.Base;
using Enemy.Types.SkeletonMage.States;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class SkeletonMage : EnemyBase {
        private MageIdleState _idleState;
        private TeleportState _teleportState;
        private CastState _castState;

        [SerializeField] private EnemyLightningStrike _enemyAttack;
        
        private const string IdleAnimationKey = "Skele Idle";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _teleportAnimationStartKey;
        [SerializeField] private AnimationClip _teleportAnimationEndKey;

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        
        protected override void Awake() {
            base.Awake();
            
            _idleState = new MageIdleState(EnemyStateMachine, this, null, 2, IdleAnimationKey);
            EnemyStateMachine.Initialize(_idleState);
        }

        protected void Start() {
            _teleportState = new TeleportState(EnemyStateMachine, this,
                new EnemyState[] { _idleState }, _teleportAnimationStartKey,
                _teleportAnimationEndKey);

            _castState =
                new CastState(EnemyStateMachine, this, new EnemyState[] { _teleportState },
                    _enemyAttack, _attackStateEnter.name, _attackStateLoop.name, _attackStateExit);
            
            _idleState.SetOutStates(new EnemyState[] { _castState, _teleportState });
        }
        
        protected override void EnterParriedState() { }

        protected override void UpdateMoveSpeedOnCharacter() { }

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