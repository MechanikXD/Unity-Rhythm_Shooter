using Core.Music;
using Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonArcher.States {
    public class Reposition : EnemyState<SkeletonArcher> {
        private Vector3 _targetPosition;

        public Reposition(SkeletonArcher enemy) : base(enemy) { }
        
        public override void EnterState() {
            Enemy.Agent.speed = Enemy.CurrentSpeed;
            Enemy.Rotation.SetDefaultMode();
            
            var newPosition = FindRandomVisiblePosition(Enemy.RepositionBounds.x, Enemy.RepositionBounds.y);
            if (newPosition == Vector3.zero) {
                ChangeState<Idle>();
                return;
            }
            
            Enemy.PlayAnimation(Enemy.RunAnimationKey);
            _targetPosition = newPosition;
            Enemy.Agent.SetDestination(_targetPosition);
            Conductor.NextBeat += Enemy.PlayWalkSound;
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_targetPosition, 0.1f)) 
                ChangeState<Idle>();
        }

        public override void ExitState() {
            Conductor.NextBeat -= Enemy.PlayWalkSound;
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