using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Interactable.AttackCollider;

namespace Enemy.Types.SkeletonTank.States {
    public class Attack : EnemyState {
        private readonly ActionSequence _attackSequence;
        
        public Attack(EnemyBase enemy, string windUpAnimKey, string attackAnimKey,
            EnemyAttackCollider collider) : base(enemy) {

            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(windUpAnimKey);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                collider.Enable();
                Enemy.PlayAnimation(attackAnimKey);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                collider.Disable();
                ChangeState<Shielding>();
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