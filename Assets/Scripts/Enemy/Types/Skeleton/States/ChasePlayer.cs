using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Enemy.Base;
using Enemy.States.Base;
using Interactable.Damageable;
using UnityEngine;

namespace Enemy.Types.Skeleton.States {
    public class ChasePlayer : EnemyState {
        private readonly DamageableBehaviour _playerRef;
        private readonly string _walkAnimationKey;

        public ChasePlayer(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            string walkAnimationKey) :
            base(stateMachine, enemy, outStates) {
            _playerRef = GameManager.Instance.Player;
            _walkAnimationKey = walkAnimationKey;
        }

        public override void EnterState() {
            SetMoveSpeed(Enemy.CurrentSpeed);
            Enemy.PlayAnimation(_walkAnimationKey);
        }

        public override void SetMoveSpeed(float value) => Enemy.Agent.speed = value;

        public override void FixedUpdate() {
            if (ReachedDestination(_playerRef.Position, EnemyBase.Proximity)) {
                AttachedStateMachine.ChangeState(OutStates[1]); // Combo state
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