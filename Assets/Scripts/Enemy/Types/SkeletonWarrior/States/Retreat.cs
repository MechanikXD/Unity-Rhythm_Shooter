using System.Collections;
using Core.Music;
using Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Types.SkeletonWarrior.States {
    public class Retreat : EnemyState {
        private readonly float _moveSpeedMultiplier;
        private readonly string _animationKey;
        private Vector3 _targetPosition;
        private readonly Vector2 _fleeBounds;
        private readonly AudioClip[] _stepSounds;
        
        public Retreat(EnemyBase enemy, Vector2 fleeBounds, float moveSpeedMultiplier, 
            string animationKey, AudioClip[] stepSounds) : base(enemy) {
            _stepSounds = stepSounds;
            _moveSpeedMultiplier = moveSpeedMultiplier;
            _animationKey = animationKey;
            _fleeBounds = fleeBounds;
        }

        public override void EnterState() {
            Enemy.PlayAnimation(_animationKey);
            
            Enemy.SetMoveSpeedMultiplier(Enemy.MoveSpeedMultiplier + _moveSpeedMultiplier);
            
            Enemy.Rotation.SetObservationPoint(Enemy.PlayerTransform);
            Enemy.SetMoveSpeedMultiplier(_moveSpeedMultiplier);
            FleeFromPlayer();
            Conductor.NextBeat += PlayWalkSound;
        }

        private void PlayWalkSound() {
            var randomDelay = Random.Range(Enemy.WalkSoundDelay.x, Enemy.WalkSoundDelay.y);
            Enemy.StartCoroutine(PlayWalkSoundDelayed(randomDelay));
        }

        private IEnumerator PlayWalkSoundDelayed(float delay) {
            yield return new WaitForSeconds(delay);
            Enemy.PlayRandomSound(_stepSounds);
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_targetPosition, 0.1f)) {
                ChangeState<Idle>();
            }
        }

        public override void ExitState() {
            Enemy.SetMoveSpeedMultiplier(Enemy.MoveSpeedMultiplier - _moveSpeedMultiplier);
            Conductor.NextBeat -= PlayWalkSound;
        }

        /// <summary>
        /// Sets destination for agent after finding flee position
        /// </summary>
        private void FleeFromPlayer() {
            Vector3 fleeDirection = (Enemy.Position - Enemy.PlayerTransform.position).normalized;
            Vector3 fleeTarget = FindFleePosition(fleeDirection);

            if (fleeTarget != Vector3.zero) {
                Enemy.Agent.SetDestination(fleeTarget);
                _targetPosition = fleeTarget;
            }
            else ChangeState<Idle>();
        }

        /// <summary>
        /// Build a "circle" around player where enemy will attempt to flee and pick first point.
        /// </summary>
        private Vector3 FindFleePosition(Vector3 preferredDirection, int attempts=10) {
            float GetFleeDistance() => Random.Range(_fleeBounds.x, _fleeBounds.y);

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