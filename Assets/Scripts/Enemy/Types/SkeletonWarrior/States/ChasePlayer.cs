using Enemy.Base;

namespace Enemy.Types.SkeletonWarrior.States {
    public class ChasePlayer : EnemyState {
        private readonly string _walkAnimationKey;

        public ChasePlayer(EnemyBase enemy, string walkAnimationKey) : base(enemy) {
            _walkAnimationKey = walkAnimationKey;
        }

        public override void EnterState() {
            Enemy.Agent.speed = Enemy.CurrentSpeed;
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(_walkAnimationKey);
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