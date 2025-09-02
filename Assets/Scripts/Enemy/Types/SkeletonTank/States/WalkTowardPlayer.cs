using Enemy.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class WalkTowardPlayer : EnemyState {
        private readonly string _walkAnimationKey;
        private readonly float _moveSpeed;

        public WalkTowardPlayer(EnemyBase enemy, float moveSpeed, string animationKey) : base(enemy) {
            _walkAnimationKey = animationKey;
            _moveSpeed = moveSpeed;
        }
        
        public override void EnterState() {
            Enemy.Agent.speed = _moveSpeed;
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(_walkAnimationKey);
        }

        public override void ExitState() {
            Enemy.Agent.SetDestination(Enemy.Position);
        }

        public override void FixedUpdate() {
            var distanceToPlayer = Enemy.PlayerDistance;
            
            if (distanceToPlayer < EnemyBase.PlayerProximity + 1f) ChangeState<AttackState>();
            else Enemy.Agent.SetDestination(Enemy.PlayerTransform.position);
        }
    }
}