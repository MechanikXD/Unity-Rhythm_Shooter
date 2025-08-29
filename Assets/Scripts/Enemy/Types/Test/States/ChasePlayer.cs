using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Enemy.Base;
using Enemy.States.Base;
using Interactable.Damageable;
using UnityEngine;

namespace Enemy.Types.Test.States {
    public class ChasePlayer : EnemyState {
        private readonly DamageableBehaviour _playerRef;

        public ChasePlayer(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates) :
            base(stateMachine, enemy, outStates) {
            _playerRef = GameManager.Instance.Player;
        }

        public override void EnterState() => SetMoveSpeed(Enemy.CurrentSpeed);

        public override void SetMoveSpeed(float value) => Enemy.Agent.speed = value;

        public override void FixedUpdate() {
            if (ReachedDestination(_playerRef.Position, EnemyBase.PlayerProximity)) {
                AttachedStateMachine.ChangeState(OutStates[0]);
            }
            else {
                Enemy.Agent.SetDestination(_playerRef.Position);
            }
        }

        private bool ReachedDestination(Vector3 point, float proximity) {
            return Vector3.Distance(Enemy.Position, point) < proximity;
        }

    }
}