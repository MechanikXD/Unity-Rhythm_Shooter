using System.Collections;
using Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonMage.States {
    public class Teleport : EnemyState {
        private readonly AnimationClip _animationEnter;
        private readonly AnimationClip _animationExit;
        private readonly Vector2 _positionBounds;

        public Teleport(EnemyBase enemy, Vector2 positionBounds, AnimationClip animationEnter, 
            AnimationClip animationExit) : base(enemy) {
            _animationEnter = animationEnter;
            _animationExit = animationExit;
            _positionBounds = positionBounds;
        }

        public override void EnterState() {
            var newPosition = FindRandomVisiblePosition(_positionBounds.x, _positionBounds.y);
            if (newPosition == Vector3.zero) {
                ChangeState<Idle>();
                return;
            }
            
            Enemy.PlayAnimation(_animationEnter.name);

            IEnumerator AfterAnimationFinished() {
                yield return new WaitForSeconds(_animationEnter.length);
                Enemy.Agent.Warp(newPosition);
                Enemy.PlayAnimation(_animationExit.name);
                Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
                
                yield return new WaitForSeconds(_animationExit.length);
                ChangeState<Idle>();
            }

            Enemy.StartCoroutine(AfterAnimationFinished());
        }

        /// <summary>
        /// Finds position at least minDist from player and at most maxDist away where player is visible
        /// </summary>
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