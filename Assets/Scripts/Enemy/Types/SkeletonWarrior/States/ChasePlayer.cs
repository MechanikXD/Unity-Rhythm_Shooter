using Core.Music;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior.States {
    public class ChasePlayer : EnemyState {
        private readonly string _walkAnimationKey;
        private readonly AudioClip[] _stepSounds;

        public ChasePlayer(EnemyBase enemy, string walkAnimationKey, AudioClip[] stepSounds) : base(enemy) {
            _walkAnimationKey = walkAnimationKey;
            _stepSounds = stepSounds;
        }

        public override void EnterState() {
            Enemy.Agent.speed = Enemy.CurrentSpeed;
            Enemy.Rotation.SetDefaultMode();
            Enemy.PlayAnimation(_walkAnimationKey);
            Conductor.NextBeat += PlayWalkSound;
        }
        
        private void PlayWalkSound() {
            Enemy.PlayRandomSound(_stepSounds);
        }

        public override void ExitState() {
            Conductor.NextBeat -= PlayWalkSound;
        }

        public override void FixedUpdate() {
            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity)) {
                ChangeState<Attack>();
            }
            else {
                Enemy.Agent.SetDestination(Enemy.PlayerTransform.position);
            }
        }
    }
}