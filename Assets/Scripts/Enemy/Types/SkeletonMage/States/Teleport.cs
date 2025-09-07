using System.Collections;
using Core.Game.VisualFX;
using Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonMage.States {
    public class Teleport : EnemyState<SkeletonMage> {
        public Teleport(SkeletonMage enemy) : base(enemy) { }

        public override void EnterState() {
            var newPosition = FindRandomVisiblePosition(Enemy.TeleportBounds.x, Enemy.TeleportBounds.y);
            if (newPosition == Vector3.zero) {
                ChangeState<Idle>();
                return;
            }
            
            VFXManager.Instance.PlayParticles(Enemy.TeleportParticle, Enemy.Position);
            Enemy.PlayRandomSound(Enemy.TeleportEnterSounds);
            Enemy.PlayAnimation(Enemy.TeleportAnimationStart.name);

            IEnumerator AfterAnimationFinished() {
                yield return new WaitForSeconds(Enemy.TeleportAnimationStart.length);
                Enemy.Agent.Warp(newPosition);
                VFXManager.Instance.PlayParticles(Enemy.TeleportParticle, Enemy.Position);
                Enemy.PlayAnimation(Enemy.TeleportAnimationEnd.name);
                Enemy.PlayRandomSound(Enemy.TeleportExitSounds);
                Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
                
                yield return new WaitForSeconds(Enemy.TeleportAnimationEnd.length);
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