using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.States.General {
    public class GoToDestination : EnemyState {
        private Vector3 _destination;
        private float _moveSpeed;
        private const float Proximity = 0.1f;

        public GoToDestination(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            float moveSpeed) : base(stateMachine, enemy, outStates) {
            _moveSpeed = moveSpeed;
        }

        public void SetDestination(Vector3 destination) {
            _destination = destination;
        }
        
        public override void EnterState() {
            Enemy.Agent.speed = _moveSpeed;
            Enemy.Agent.SetDestination(_destination);
        }

        public override void FrameUpdate() {
            if (ReachedDestination(_destination, Proximity)) {
                AttachedStateMachine.ChangeState(OutStates[0]);
            }
        }

        private bool ReachedDestination(Vector3 point, float proximity) {
            return Vector3.Distance(Enemy.Position, point) < proximity;
        }
    }
}