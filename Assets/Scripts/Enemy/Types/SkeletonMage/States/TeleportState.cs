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
            var newPosition = FindVisiblePosition(15f);
            if (newPosition == Vector3.zero) {
                AttachedStateMachine.ChangeState(OutStates[0]); // Idle state
                return;
            }
            
            Enemy.PlayAnimation(_animationEnter.name);

            IEnumerator AfterAnimationFinished() {
                yield return new WaitForSeconds(_animationEnter.length);
                Enemy.Agent.Warp(newPosition);
                Enemy.PlayAnimation(_animationExit.name);
                
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

                if (Enemy.HasLineOfSightWithPlayer()) return hit.position;
            }

            return Vector3.zero;
        }
    }
}