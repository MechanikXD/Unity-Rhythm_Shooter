using Core.Music;
using Enemy.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class ChasePlayer : EnemyState {
        private const float DistCorrection = 1f;  // Due to enemy size, distance should be adjusted 
        private readonly string _walkAnimationKey;

        public ChasePlayer(EnemyBase enemy, string animationKey) : base(enemy) {
            _walkAnimationKey = animationKey;
        }
        
        public override void EnterState() {
            Enemy.Agent.speed = Enemy.CurrentSpeed;
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(_walkAnimationKey);
            Conductor.NextBeat += Enemy.PlayWalkSound;
        }

        public override void ExitState() {
            Enemy.Agent.SetDestination(Enemy.Position);
            Conductor.NextBeat -= Enemy.PlayWalkSound;
        }

        public override void FixedUpdate() {
            if (Enemy.DistanceToPlayer < EnemyBase.PlayerProximity + DistCorrection) 
                ChangeState<Attack>();
            else Enemy.Agent.SetDestination(Enemy.PlayerTransform.position);
        }
    }
}