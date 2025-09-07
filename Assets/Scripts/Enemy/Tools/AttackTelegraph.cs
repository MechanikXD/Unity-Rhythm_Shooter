using Core.Game.Audio;
using Core.Game.VisualFX;
using UnityEngine;

namespace Enemy.Tools {
    public class AttackTelegraph : MonoBehaviour {
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private AudioClip _sound;
        [SerializeField] private float _pitch;
        [SerializeField] private float _soundDistance;

        public ParticleSystem Particle => _particle;
        
        public void Play() {
            var position = transform.position;
            
            VFXManager.Instance.PlayParticles(_particle, position);
            AudioManager.Instance.PlaySound(_sound, position, _soundDistance, _pitch);
            
            Destroy(gameObject, Mathf.Max(_sound.length, _particle.main.duration));
        }
    }
}