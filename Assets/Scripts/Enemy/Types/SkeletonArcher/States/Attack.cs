using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher.States {
    public class Attack : EnemyState<SkeletonArcher> {
        private readonly ActionSequence _attackSequence;
        private EnemyArrow _lastCreatedArrow;

        public Attack(SkeletonArcher enemy) : base(enemy) {
            var exitAnimation = Enemy.AttackStateExit;
            var lockPosition = Vector3.zero;
            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayRandomSound(Enemy.ArrowLoadSounds);
                // Wait one beat
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(Enemy.AttackStateLoop.name);
                lockPosition = Enemy.DirectionToPlayer;
                lockPosition.y -= Enemy.ArrowHeightCorrection;

                // TODO: Create attack indicator
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayRandomSound(Enemy.ArrowReleaseSounds);
                if (_lastCreatedArrow != null) _lastCreatedArrow.Launch(Enemy, lockPosition);

                Enemy.PlayAnimation(exitAnimation.name);
                Enemy.StartCoroutine(
                    ForceExitStateAfter(exitAnimation.length, typeof(Reposition)));            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
            Enemy.PlayAnimation(Enemy.AttackStateEnter.name);
            
            _lastCreatedArrow = Object.Instantiate(Enemy.ArrowPrefab, Enemy.ArrowSpawnPoint);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }
    }
}