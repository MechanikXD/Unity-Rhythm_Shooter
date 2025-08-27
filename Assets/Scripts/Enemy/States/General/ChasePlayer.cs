using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Enemy.Base;
using Enemy.States.Base;
using Interactable.Damageable;
using UnityEngine;

namespace Enemy.States.General {
    public class ChasePlayer : EnemyState {
        private DamageableBehaviour _playerRef;
        private float _moveSpeed;
        private const float Proximity = 0.1f;

        public ChasePlayer(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates, 
            float moveSpeed) : base(stateMachine, enemy, outStates) {
            _moveSpeed = moveSpeed;
        }
        public override void EnterState() {
            _playerRef = GameManager.Instance.Player;
            Enemy.Agent.speed = _moveSpeed;
        }

        public override void FixedUpdate() {
            if (ReachedDestination(_playerRef.Position, Proximity)) {
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