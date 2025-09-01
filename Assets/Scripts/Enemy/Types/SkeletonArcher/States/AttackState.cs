using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher.States {
    public class AttackState : EnemyState {
        private readonly Transform _arrowSpawnPoint;
        private readonly EnemyArrow _arrow;
        
        private readonly string _enterAnimationKey;
        private readonly ActionSequence _attackSequence;
        EnemyArrow _lastCreatedArrow;

        public AttackState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            Transform arrowSpawnPoint, EnemyArrow arrow, string stateStartKey, string stateLoopKey,
            AnimationClip stateExit) : base(stateMachine, enemy, outStates) {
            
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
                lockPosition = Enemy.PlayerDirection;
                lockPosition.y -= 0.2f;

                // TODO: Create attack indicator
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                if (_lastCreatedArrow != null) _lastCreatedArrow.Launch(Enemy, lockPosition);

                Enemy.PlayAnimation(exitAnimation.name);
                Enemy.StartCoroutine(ForceExitStateAfter(exitAnimation.length, 0)); // Reposition state
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }
        
        private IEnumerator ForceExitStateAfter(float delay, int stateIndex) {
            yield return new WaitForSeconds(delay);
            AttachedStateMachine.ChangeState(OutStates[stateIndex]);
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