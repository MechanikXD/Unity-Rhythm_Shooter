using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.States.Shared {
    public class LookForPlayer : EnemyState {
        private readonly float _moveSpeed;
        private bool _destinationWasSet;
        
        public LookForPlayer(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            float moveSpeed) : base(stateMachine, enemy, outStates) {
            _moveSpeed = moveSpeed;
        }

        public override void EnterState() {
            Enemy.Agent.speed = _moveSpeed;
            _destinationWasSet = false;
            Enemy.Rotation.SetDefaultMode();
            FindVantagePoint();
        }

        public override void FrameUpdate() {
            if (_destinationWasSet && !Enemy.AgentAtDestination()) return; 
            
            AttachedStateMachine.ChangeState(HasLineOfSight(Enemy.Position, Enemy.PlayerTransform.position)
                ? OutStates[1] // Attack state
                : this); // Re-enter state
        }

        private void FindVantagePoint() {
            Vector3 vantagePoint = FindVisiblePosition(15f);

            if (vantagePoint != Vector3.zero) {
                Enemy.Agent.SetDestination(vantagePoint);
                _destinationWasSet = true;
            }
            else {
                AttachedStateMachine.ChangeState(OutStates[0]); // Chase Player
            }
        }

        private Vector3 FindVisiblePosition(float searchRadius, int attempts = 16) {
            for (var i = 0; i < attempts; i++) {
                // Create circle of potential positions around player
                var angle = i * (360f / attempts);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 testPosition = Enemy.PlayerTransform.position + direction * searchRadius;

                if (!NavMesh.SamplePosition(testPosition, out var hit, 5f, NavMesh.AllAreas)) {
                    continue;
                }

                if (HasLineOfSight(hit.position, Enemy.PlayerTransform.position)) return hit.position;
            }

            return Vector3.zero;
        }

        private static bool HasLineOfSight(Vector3 from, Vector3 to) {
            var distance = Vector3.Distance(from, to);
            // RayCast from eye level
            var eyeLevel = from + Vector3.up * 1.5f;
            var targetEyeLevel = to + Vector3.up * 1.5f;
            
            return Physics.Raycast(eyeLevel, (targetEyeLevel - eyeLevel).normalized, out var hit,
                distance) && hit.transform.gameObject.TryGetComponent<PlayerController>(out _);
        }
    }
}