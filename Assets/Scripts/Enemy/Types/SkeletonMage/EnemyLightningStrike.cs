using Core.Game.Audio;
using Core.Game.VisualFX;
using Enemy.Base;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class EnemyLightningStrike : MonoBehaviour {
        [SerializeField] private ParticleSystem _magicCircle;
        [SerializeField] private AudioClip _prepareSound;
        [SerializeField] private float _prepareSoundReach;
        
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private AudioClip _attackSound;
        [SerializeField] private float _attackSoundReach;
        
        [SerializeField] private EnemyAttackCollider _attackCollider;
        private ParticleSystem _activeMagicCircle;
        
        public ParticleSystem[] Particles => new []{ _particle, _magicCircle };
        
        private void Awake() {
            var currentPosition = transform.position;
            AudioManager.Instance.PlaySound(_prepareSound, currentPosition, _prepareSoundReach);
            _activeMagicCircle = VFXManager.Instance.GetParticles(_magicCircle, currentPosition, false);
            _attackCollider.Disable();
        }

        public void Launch(EnemyBase owner) {
            _activeMagicCircle.gameObject.SetActive(false);
            _activeMagicCircle = null;
            
            _attackCollider.SetOwner(owner);
            var position = transform.position;
            VFXManager.Instance.PlayParticles(_particle, position);
            AudioManager.Instance.PlaySound(_attackSound, position, _attackSoundReach);
            _attackCollider.Enable();
            
            Destroy(gameObject, _particle.main.duration);
        }

        private void OnDestroy() => _attackCollider.Disable();
    }
}