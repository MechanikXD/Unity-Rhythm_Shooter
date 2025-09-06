using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class Attack : EnemyState<SkeletonTank> {
        private readonly ActionSequence _attackSequence;
        
        public Attack(SkeletonTank enemy) : base(enemy) {
            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(Enemy.AttackWindUpAnimation);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.AttackCollider.Enable();
                Enemy.PlayRandomSound(Enemy.SwingSounds);
                Enemy.PlayAnimation(Enemy.AttackAnimation);
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                enemy.AttackCollider.Disable();
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