using Enemy.Base;

namespace Enemy.Types.SkeletonTank {
    public class SkeletonTank : EnemyBase {
        private const string IdleAnimationKey = "Skele Idle";
        private const string DeathAnimationKey = "Skely Death";
        
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