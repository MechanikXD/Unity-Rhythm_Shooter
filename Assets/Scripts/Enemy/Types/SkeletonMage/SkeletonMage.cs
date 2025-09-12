using Core.Behaviour.FiniteStateMachine;
using Core.Game.VisualFX;
using Enemy.Base;
using Enemy.Types.SkeletonMage.States;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class SkeletonMage : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private EnemyLightningStrike _enemyAttack;

        public EnemyLightningStrike LightningStrike => _enemyAttack;

        [Header("Behaviour:")]
        [SerializeField] private float _idleTime = 3f;
        [SerializeField] private Vector2 _teleportBounds = new Vector2(5f, 10f);
        [SerializeField] private int _attackCount = 3;

        public float IdleTime => _idleTime;
        public Vector2 TeleportBounds => _teleportBounds;
        public int AttackCount => _attackCount;
        
        [Header("Animations:")]
        [SerializeField] private string _idleAnimationKey = "Mage Idle";
        [SerializeField] private AnimationClip _deathAnimation;
        
        [SerializeField] private AnimationClip _teleportAnimationStart;
        [SerializeField] private AnimationClip _teleportAnimationEnd;

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        
        public string IdleAnimationKey => _idleAnimationKey;
        public AnimationClip TeleportAnimationStart => _teleportAnimationStart;
        public AnimationClip TeleportAnimationEnd => _teleportAnimationEnd;
        public AnimationClip AttackStateEnter => _attackStateEnter;
        public AnimationClip AttackStateLoop => _attackStateLoop;
        public AnimationClip AttackStateExit => _attackStateExit;
        
        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _teleportEnterSounds;
        [SerializeField] private AudioClip[] _teleportExitSounds;
        
        public AudioClip[] TeleportEnterSounds => _teleportEnterSounds;
        public AudioClip[] TeleportExitSounds => _teleportExitSounds;
        
        [Header("Particle")]
        [SerializeField] private ParticleSystem _teleportParticle;

        public ParticleSystem TeleportParticle => _teleportParticle;

        protected override State[] InitializeStates() {
            var idleState = new Idle(this);
            var teleportState = new Teleport(this);
            var castState = new Cast(this);

            foreach (var particle in _enemyAttack.Particles) {
                VFXManager.Instance.RegisterParticles(particle, 4);    
            }
            VFXManager.Instance.RegisterParticles(_teleportParticle, 4);
            
            return new State[] {
                idleState,
                teleportState,
                castState
            };
        }

        protected override void UpdateAnimationSpeed() { }

        public override void Die() {
            base.Die();
            StateMachine.StopMachine();
            _animator.CrossFade(_deathAnimation.name, _crossFade, -1, 0f);
            
            Destroy(gameObject, _deathAnimation.length);
        }
    }
}