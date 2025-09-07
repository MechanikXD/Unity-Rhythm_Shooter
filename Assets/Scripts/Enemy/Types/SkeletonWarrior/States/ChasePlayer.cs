using Core.Music;
using Enemy.Base;

namespace Enemy.Types.SkeletonWarrior.States {
    public class ChasePlayer : EnemyState<SkeletonWarrior> {
        public ChasePlayer(SkeletonWarrior enemy) : base(enemy) { }

        public override void EnterState() {
            Enemy.Agent.speed = Enemy.CurrentSpeed;
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(Enemy.WalkAnimationKey);
            Conductor.NextBeat += Enemy.PlayWalkSound;
        }

        public override void ExitState() {
            Conductor.NextBeat -= Enemy.PlayWalkSound;
        }

        public override void FixedUpdate() {
            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity)) {
                ChangeState<Attack>();
            }
            else {
                Enemy.Agent.SetDestination(Enemy.PlayerTransform.position);
            }
        }
    }
}