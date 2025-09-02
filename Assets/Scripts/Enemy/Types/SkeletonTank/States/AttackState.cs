using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class AttackState : EnemyState {
        private readonly ActionSequence _attackSequence;
        
        public AttackState(EnemyBase enemy, AnimationClip windUp, AnimationClip attack,
            EnemyAttackCollider collider) : base(enemy) {

            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(windUp.name);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                collider.Enable();
                Enemy.PlayAnimation(attack.name);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                collider.Disable();
                ChangeState<ShieldIdle>();
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }
        
        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }
    }
}