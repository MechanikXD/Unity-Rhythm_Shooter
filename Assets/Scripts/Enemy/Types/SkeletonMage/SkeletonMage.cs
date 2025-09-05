using Core.Game.VisualFX;
using Enemy.Base;
using Enemy.Types.SkeletonMage.States;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class SkeletonMage : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private EnemyLightningStrike _enemyAttack;

        [Header("Behaviour:")]
        [SerializeField] private Vector2 _teleportBounds = new Vector2(5f, 10f);
        [SerializeField] private int _attackCount = 3;
        
        [Header("Animations:")]
        private const string IdleAnimationKey = "Mage Idle";
        private const string DeathAnimationKey = "Skely Death";
        
        [SerializeField] private AnimationClip _teleportAnimationStartKey;
        [SerializeField] private AnimationClip _teleportAnimationEndKey;

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        
        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _teleportEnterSounds;
        [SerializeField] private AudioClip[] _teleportExitSounds;
        
        [Header("Particle")]
        [SerializeField] private ParticleSystem _teleportParticle;

        protected override EnemyState[] InitializeStates() {
            var idleState = new Idle(this, 2, IdleAnimationKey);
            var teleportState = new Teleport(this, _teleportBounds,
                _teleportAnimationStartKey, _teleportAnimationEndKey,
                _teleportEnterSounds, _teleportExitSounds, _teleportParticle);
            var castState = new Cast(this, _attackCount, _enemyAttack, 
                _attackStateEnter.name, _attackStateLoop.name, _attackStateExit);

            foreach (var particle in _enemyAttack.Particles) {
                VFXManager.Instance.RegisterParticles(particle, 4);    
            }
            VFXManager.Instance.RegisterParticles(_teleportParticle, 4);
            
            return new EnemyState[] {
                idleState,
                teleportState,
                castState
            };
        }

        protected override void UpdateAnimationSpeed() { }

        public override void Die() {
            base.Die();
            _animator.CrossFade(DeathAnimationKey, _crossFade, -1, 0f);
            
            Destroy(gameObject, DeathAnimationKey.Length);
        }
    }
}