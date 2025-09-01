using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class CastState : EnemyState {
        private readonly Transform _player;
        private readonly EnemyLightningStrike _enemyAttack;
        private const int AttackCount = 3;

        private readonly string _enterAnimationKey;
        
        private readonly ActionSequence _attackSequence;
        private readonly LayerMask _playerMask;

        public CastState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            EnemyLightningStrike enemyAttack, string stateEnterKey, string stateLoopKey, AnimationClip stateExit)
            : base(stateMachine, enemy, outStates) {
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
                Enemy.StartCoroutine(ForceExitStateAfter(stateExit.length, 0));
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }
        
        private IEnumerator ForceExitStateAfter(float delay, int stateIndex) {
            yield return new WaitForSeconds(delay);
            AttachedStateMachine.ChangeState(OutStates[stateIndex]);
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