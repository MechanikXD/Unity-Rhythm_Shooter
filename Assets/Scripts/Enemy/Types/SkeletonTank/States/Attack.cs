using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;

namespace Enemy.Types.SkeletonTank.States {
    public class Attack : EnemyState<SkeletonTank> {
        private readonly ActionSequence _attackSequence;

        public Attack(SkeletonTank enemy) : base(enemy) {
            var sequenceBuilder = new ActionSequenceBuilder();
            
            sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                Enemy.PlayAnimation(Enemy.AttackWindUpAnimation);
                Enemy.PlayAttackTelegraphParticle(Enemy.AxePosition);
                Enemy.Trail.enabled = true;
            });
            sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                Enemy.AttackCollider.Enable();
                Enemy.PlayRandomSound(Enemy.SwingSounds);
                Enemy.PlayAnimation(Enemy.AttackAnimation);
            });
            sequenceBuilder.Append(Trigger.NextBeat, SequenceProtector, _ => {
                enemy.AttackCollider.Disable();
                ChangeState<Shielding>();
                Enemy.Trail.enabled = false;
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        private bool SequenceProtector()
        {
            return Enemy != null;
        }
        
        public override void EnterState() {
            PlayOrRestartSequence();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
        }

        public override void ExitState()
        {
            if (!_attackSequence.IsFinished) _attackSequence.Break();
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }
    }
}