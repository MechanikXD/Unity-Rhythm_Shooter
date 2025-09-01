using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;

namespace Enemy.States.Shared {
    public class ChasePlayer : EnemyState {
        private readonly string _walkAnimationKey;

        public ChasePlayer(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            string walkAnimationKey) :
            base(stateMachine, enemy, outStates) {
            _walkAnimationKey = walkAnimationKey;
        }

        public override void EnterState() {
            SetMoveSpeed(Enemy.CurrentSpeed);
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(_walkAnimationKey);
        }

        public override void SetMoveSpeed(float value) => Enemy.Agent.speed = value;

        public override void FixedUpdate() {
            if (Enemy.NearPoint(Enemy.PlayerTransform.position, EnemyBase.PlayerProximity)) {
                AttachedStateMachine.ChangeState(OutStates[0]); // Attack state
            }
            else {
                Enemy.Agent.SetDestination(Enemy.PlayerTransform.position);
            }
        }
    }
}