using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher.States {
    public class Attack : EnemyState {
        private readonly Transform _arrowSpawnPoint;
        private readonly EnemyArrow _arrow;
        
        private readonly string _enterAnimationKey;
        private readonly ActionSequence _attackSequence;
        EnemyArrow _lastCreatedArrow;

        public Attack(EnemyBase enemy, Transform arrowSpawnPoint, EnemyArrow arrow,
            float arrayHeightCorrection, string stateStartKey, string stateLoopKey,
            AnimationClip stateExit) : base(enemy) {
            
            var exitAnimation = stateExit;
            _enterAnimationKey = stateStartKey;
            _arrowSpawnPoint = arrowSpawnPoint;
            _arrow = arrow;
            
            Vector3 lockPosition = Vector3.zero;

            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                // Wait one beat
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(stateLoopKey);
                lockPosition = Enemy.DirectionToPlayer;
                lockPosition.y -= arrayHeightCorrection;

                // TODO: Create attack indicator
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                if (_lastCreatedArrow != null) _lastCreatedArrow.Launch(Enemy, lockPosition);

                Enemy.PlayAnimation(exitAnimation.name);
                Enemy.StartCoroutine(
                    ForceExitStateAfter(exitAnimation.length, typeof(Reposition)));            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
            Enemy.PlayAnimation(_enterAnimationKey);
            
            _lastCreatedArrow = Object.Instantiate(_arrow, _arrowSpawnPoint);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }
    }
}