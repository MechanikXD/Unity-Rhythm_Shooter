using System.Collections.Generic;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Interactable.AttackCollider;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Types.SkeletonWarrior.States {
    public class Attack : EnemyState {
        private readonly ActionSequence _comboSequence;
        private readonly EnemyAttackCollider _attackCollider;
        private readonly string _windUpAnimationKey;
        private Vector3 _destination;
        private readonly AudioClip[] _swings;
        
        public Attack(EnemyBase enemy, IReadOnlyList<AnimationClip> animations, 
            EnemyAttackCollider attackCollider, IReadOnlyList<float> forwardMovement,
            AudioClip[] attackSounds) : base(enemy) {
            _swings = attackSounds;
            _windUpAnimationKey = animations[0].name;
            _attackCollider = attackCollider;
            var attackAnimations = new[] { animations[1], animations[2], animations[3] };
            
            var sequenceBuilder = new ActionSequenceBuilder();

            for (var i = 0; i < attackAnimations.Length; i++) {
                var index = i;
                sequenceBuilder.Append(Trigger.NextBeat, _ => {
                    AttackPlayer(attackAnimations[index].name, forwardMovement[index]);
                });
            }
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                AttackPlayer(animations[4].name, forwardMovement[3]);
                
                // Random 50/50 between idle and retreat
                var outState = Random.value <= 0.5f ? typeof(Idle) : typeof(Retreat);
                Enemy.StartCoroutine(ForceExitStateAfter(animations[4].length, outState));
            });

            _comboSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            _destination = Enemy.Position;
            PlayOrRestartSequence();
            _attackCollider.Enable();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
            Enemy.PlayAnimation(_windUpAnimationKey);
        }

        private void PlayOrRestartSequence() {
            if (_comboSequence.IsFinished) _comboSequence.Restart();
            else _comboSequence.Start();
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_destination, 0.1f)) return;

            Enemy.transform.position = Vector3.MoveTowards(Enemy.Position, _destination,
                Enemy.CurrentSpeed * Time.deltaTime);
        }

        public override void ExitState() {
            _attackCollider.Disable();
        }

        private void AttackPlayer(string animationKey, float forwardMovement) {
            Enemy.PlayAnimation(animationKey);
            Enemy.PlayRandomSound(_swings);
            _destination += Enemy.Forward * forwardMovement;
            _attackCollider.Reset();
        }
    }
}