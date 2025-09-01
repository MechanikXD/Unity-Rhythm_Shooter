using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.States.Shared {
    public class StepBack : EnemyState {
        private readonly float _moveSpeed;
        private readonly string _animationKey;
        private Vector3 _targetPosition;
        
        public StepBack(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            float moveSpeed, string animationKey)
            : base(stateMachine, enemy, outStates) {
            _moveSpeed = moveSpeed;
            _animationKey = animationKey;
        }

        public override void EnterState() {
            Enemy.PlayAnimation(_animationKey);
            Enemy.Rotation.SetObservationPoint(Enemy.PlayerTransform);
            Enemy.Agent.speed = _moveSpeed;
            FleeFromPlayer();
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_targetPosition, 0.5f)) {
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
            }
        }

        private void FleeFromPlayer() {
            Vector3 fleeDirection = (Enemy.Position - Enemy.PlayerTransform.position).normalized;
            Vector3 fleeTarget = FindFleePosition(fleeDirection);

            if (fleeTarget != Vector3.zero) {
                Enemy.Agent.SetDestination(fleeTarget);
                _targetPosition = fleeTarget;
            }
            else {
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
            }
        }

        private Vector3 FindFleePosition(Vector3 preferredDirection, int attempts=10) {
            float GetFleeDistance() => Random.Range(2f, 6f);

            for (var i = 0; i < attempts; i++) {
                // Create arc of potential flee points
                var angle = i * 45f - 180f; // Spread around behind enemy
                Vector3 direction = Quaternion.Euler(0, angle, 0) * preferredDirection;
                Vector3 testPosition = Enemy.Position + direction * GetFleeDistance();
                
                if (!NavMesh.SamplePosition(testPosition, out var hit, 5f, NavMesh.AllAreas)) {
                    continue;
                }

                // Check if this position is actually farther from player
                if (Vector3.Distance(hit.position, Enemy.PlayerTransform.position) >
                    Vector3.Distance(Enemy.Position, Enemy.PlayerTransform.position)) {
                    return hit.position;
                }
            }

            return Vector3.zero; // No suitable position found
        }
    }
}