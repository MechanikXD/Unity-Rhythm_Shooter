using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.Test.States {
    public class GoToDestination : EnemyState {
        private Vector3 _destination;

        public GoToDestination(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates) 
            : base(stateMachine, enemy, outStates) { }

        public void SetDestination(Vector3 destination) => _destination = destination;
        
        public override void EnterState() {
            Enemy.Agent.SetDestination(_destination);
            SetMoveSpeed(Enemy.CurrentSpeed);
        }

        public override void SetMoveSpeed(float value) => Enemy.Agent.speed = value;

        public override void FrameUpdate() {
            if (ReachedDestination(_destination, EnemyBase.Proximity)) {
                AttachedStateMachine.ChangeState(OutStates[0]);
            }
        }

        private bool ReachedDestination(Vector3 point, float proximity) {
            return Vector3.Distance(Enemy.Position, point) < proximity;
        }
    }
}