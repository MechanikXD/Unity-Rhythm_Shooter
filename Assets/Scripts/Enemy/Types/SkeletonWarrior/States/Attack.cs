using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Types.SkeletonWarrior.States {
    public class Attack : EnemyState<SkeletonWarrior> {
        private readonly ActionSequence _attackSequence;
        private Vector3 _destination;
        
        public Attack(SkeletonWarrior enemy) : base(enemy) {
            var sequenceBuilder = new ActionSequenceBuilder();
            const int attackCount = 3;
            
            for (var i = 0; i < attackCount; i++) {
                var index = i;
                sequenceBuilder.Append(Trigger.NextBeat, _ => {
                    AttackPlayer(Enemy.AttackAnimations[index + 1].name, Enemy.ForwardMovementDuringAttack[index]);
                });
            }
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                AttackPlayer(Enemy.AttackAnimations[4].name, Enemy.ForwardMovementDuringAttack[3]);
                
                // Random 50/50 between idle and retreat
                var outState = Random.value <= 0.5f ? typeof(Idle) : typeof(Retreat);
                Enemy.StartCoroutine(ForceExitStateAfter(Enemy.AttackAnimations[4].length, outState));
            });

            _attackSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            _destination = Enemy.Position;
            PlayOrRestartSequence();
            Enemy.AttackCollider.Enable();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
            Enemy.PlayAnimation(Enemy.AttackAnimations[0].name);
        }

        private void PlayOrRestartSequence() {
            if (_attackSequence.IsFinished) _attackSequence.Restart();
            else _attackSequence.Start();
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_destination, 0.1f)) return;

            Enemy.transform.position = Vector3.MoveTowards(Enemy.Position, _destination,
                Enemy.CurrentSpeed * Time.deltaTime);
        }

        public override void ExitState() {
            Enemy.AttackCollider.Disable();
        }

        private void AttackPlayer(string animationKey, float forwardMovement) {
            Enemy.PlayAnimation(animationKey);
            Enemy.PlayRandomSound(Enemy.SwingSounds);
            _destination += Enemy.Forward * forwardMovement;
            Enemy.AttackCollider.Reset();
        }
    }
}