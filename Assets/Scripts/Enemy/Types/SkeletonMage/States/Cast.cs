using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class Cast : EnemyState<SkeletonMage> {
        private readonly ActionSequence _attackSequence;

        public Cast(SkeletonMage enemy) : base(enemy) {
            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                Enemy.PlayAnimation(Enemy.AttackStateLoop.name);
                AttackPlayer();
            });

            for (var i = 0; i < Enemy.AttackCount - 1; i++) {
                sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                    AttackPlayer();
                });
            }
            
            sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                Enemy.PlayAnimation(Enemy.AttackStateExit.name);
                Enemy.StartCoroutine(ForceExitStateAfter(Enemy.AttackStateExit.length, typeof(Teleport)));
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        private bool SequenceProtector()
        {
            return Enemy != null;
        }

        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.SetLocalDirection(Enemy.Forward);
            Enemy.PlayAnimation(Enemy.AttackStateEnter.name);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }

        public override void ExitState()
        {
            if (!_attackSequence.IsFinished) _attackSequence.Break();
        }

        private void AttackPlayer() {
            var playerPos = Enemy.PlayerTransform.position;
            playerPos.y -= 0.9f;
            
            var newAttack =
                Object.Instantiate(Enemy.LightningStrike,  playerPos, Quaternion.identity);
            Conductor.Instance.AddOnNextBeat(() => newAttack.Launch(Enemy));
        }
    }
}