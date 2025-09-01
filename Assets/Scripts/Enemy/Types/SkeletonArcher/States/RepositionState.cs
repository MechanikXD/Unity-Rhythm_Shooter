using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonArcher.States {
    public class RepositionState : EnemyState {
        private readonly string _runAnimationKey;
        private readonly float _moveSpeed; 
        private Vector3 _targetPosition;

        public RepositionState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            string runAnimationKey, float moveSpeed)
            : base(stateMachine, enemy, outStates) {
            _moveSpeed = moveSpeed;
            _runAnimationKey = runAnimationKey;
        }
        
        public override void EnterState() {
            Enemy.Agent.speed = _moveSpeed;
            Enemy.Rotation.SetDefaultMode();
            
            var newPosition = FindRandomVisiblePosition(5f, 20f);
            if (newPosition == Vector3.zero) {
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
                return;
            }
            
            Enemy.PlayAnimation(_runAnimationKey);
            _targetPosition = newPosition;
            Enemy.Agent.SetDestination(_targetPosition);
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_targetPosition, 1f)) {
                AttachedStateMachine.ChangeState(OutStates[0]); // IdleState
            }
        }

        private Vector3 FindRandomVisiblePosition(float minDistance, float maxDistance, 
            int attempts = 16, int distanceSteps = 4) {
            
            for (int angleIndex = 0; angleIndex < attempts; angleIndex++) {
                float angle = 360f / attempts * angleIndex;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                for (int distIndex = 0; distIndex < distanceSteps; distIndex++) {
                    float distance = Mathf.Lerp(minDistance, maxDistance,
                        (float)distIndex / (distanceSteps - 1));
                    Vector3 testPosition = Enemy.PlayerTransform.position + direction * distance;

                    if (NavMesh.SamplePosition(testPosition, out var navHit, 3f, NavMesh.AllAreas)) {
                        if (navHit.position != Enemy.Position &&
                            Enemy.HasLineOfSightWithPlayer(navHit.position))
                            return navHit.position;
                    }
                }
            }

            return Vector3.zero;
        }
    }
}