using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class Cast : EnemyState {
        private readonly EnemyLightningStrike _enemyAttack;
        private readonly string _enterAnimationKey;
        private readonly ActionSequence _attackSequence;

        public Cast(EnemyBase enemy, int attackCount, EnemyLightningStrike enemyAttack,
            string stateEnterKey, string stateLoopKey, AnimationClip stateExit) : base(enemy) {
            _enemyAttack = enemyAttack;
            _enterAnimationKey = stateEnterKey;

            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(stateLoopKey);
                AttackPlayer();
            });

            for (var i = 0; i < attackCount - 1; i++) {
                sequenceBuilder.Append(Trigger.NextBeat, _ => {
                    AttackPlayer();
                });
            }
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(stateExit.name);
                Enemy.StartCoroutine(ForceExitStateAfter(stateExit.length, typeof(Teleport)));
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.SetLocalDirection(Enemy.Forward);
            Enemy.PlayAnimation(_enterAnimationKey);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }
        
        private void AttackPlayer() {
            // TODO: Create Indicator
            
            var newAttack =
                Object.Instantiate(_enemyAttack, Enemy.PlayerTransform.position, Quaternion.identity);
            Conductor.Instance.AddOnNextBeat(() => newAttack.Launch(Enemy));
        }
    }
}