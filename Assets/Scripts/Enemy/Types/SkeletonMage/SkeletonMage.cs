using Enemy.Base;
using Enemy.Types.SkeletonMage.States;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class SkeletonMage : EnemyBase {
        [SerializeField] private EnemyLightningStrike _enemyAttack;
        
        private const string IdleAnimationKey = "Mage Idle";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _teleportAnimationStartKey;
        [SerializeField] private AnimationClip _teleportAnimationEndKey;

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;

        protected override EnemyState[] InitializeStates() {
            var idleState = new MageIdleState(this, 2, IdleAnimationKey);
            var teleportState = new TeleportState(this, _teleportAnimationStartKey,
                _teleportAnimationEndKey);
            var castState =
                new CastState(this, _enemyAttack, _attackStateEnter.name, 
                    _attackStateLoop.name, _attackStateExit);

            return new EnemyState[] {
                idleState,
                teleportState,
                castState
            };
        }

        protected override void UpdateAnimationSpeed() { }

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