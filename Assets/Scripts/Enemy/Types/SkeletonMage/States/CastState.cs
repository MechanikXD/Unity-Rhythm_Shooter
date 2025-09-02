using Core.Game;
using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class CastState : EnemyState {
        private readonly Transform _player;
        private readonly EnemyLightningStrike _enemyAttack;
        private const int AttackCount = 3;

        private readonly string _enterAnimationKey;
        
        private readonly ActionSequence _attackSequence;
        private readonly LayerMask _playerMask;

        public CastState(EnemyBase enemy, EnemyLightningStrike enemyAttack,
            string stateEnterKey, string stateLoopKey, AnimationClip stateExit)
            : base(enemy) {
            _enemyAttack = enemyAttack;
            _player = GameManager.Instance.Player.transform;

            _enterAnimationKey = stateEnterKey;

            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(stateLoopKey);
                AttackPlayer();
            });

            for (var i = 0; i < AttackCount - 1; i++) {
                sequenceBuilder.Append(Trigger.NextBeat, _ => {
                    AttackPlayer();
                });
            }
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(stateExit.name);
                Enemy.StartCoroutine(ForceExitStateAfter(stateExit.length, typeof(TeleportState)));
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
                Object.Instantiate(_enemyAttack, _player.position, Quaternion.identity);
            Conductor.Instance.AddOnNextBeat(() => newAttack.Launch(Enemy));
        }
    }
}