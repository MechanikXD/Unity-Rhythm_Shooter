using Enemy.Base;
using Enemy.States.Shared;
using Enemy.Types.SkeletonTank.States;
using UnityEngine;

namespace Enemy.Types.SkeletonTank {
    public class SkeletonTank : EnemyBase {
        private IdleState _idleState;
        private WalkTowardPlayer _walkPlayer;
        [SerializeField] private float _unshieldedDistanceFromPlayer;
        private ShieldWalk _shieldWalk;
        private ShieldIdle _shieldIdle;
        
        private AttackState _attackState;
        
        private const string IdleAnimationKey = "Paladin Idle";
        private const string WalkAnimationKey = "Paladin Walk";
        private const string ShieldWalkAnimationKey = "Paladin Block";
        private const string ShieldIdleAnimationKey = "Paladin Idle";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _attackWindUp;
        [SerializeField] private AnimationClip _attack;
        
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