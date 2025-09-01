using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonMage.States {
    public class TeleportState : EnemyState {
        private readonly AnimationClip _animationEnter;
        private readonly AnimationClip _animationExit;

        public TeleportState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            AnimationClip animationEnter, AnimationClip animationExit)
            : base(stateMachine, enemy, outStates) {
            _animationEnter = animationEnter;
            _animationExit = animationExit;
        }

        public override void EnterState() {
            var newPosition = FindRandomVisiblePosition(3f, 15f);
            if (newPosition == Vector3.zero) {
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
                return;
            }
            
            Enemy.PlayAnimation(_animationEnter.name);

            IEnumerator AfterAnimationFinished() {
                yield return new WaitForSeconds(_animationEnter.length);
                Enemy.Agent.Warp(newPosition);
                Enemy.PlayAnimation(_animationExit.name);
                Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
                
                yield return new WaitForSeconds(_animationExit.length);
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
            }

            Enemy.StartCoroutine(AfterAnimationFinished());
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
                
                Debug.Log($"Point {testPosition} is on the mesh");

                if (Enemy.HasLineOfSightWithPlayer()) {
                    Debug.Log("HAS LINE OF SIGHT");
                    return hit.position;
                }
                
                Debug.Log("Has no line of sight");
            }

            return Vector3.zero;
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
                        if (Enemy.HasLineOfSightWithPlayer(navHit.position))
                            return navHit.position;
                    }
                }
            }

            return Vector3.zero;
        }
    }
}