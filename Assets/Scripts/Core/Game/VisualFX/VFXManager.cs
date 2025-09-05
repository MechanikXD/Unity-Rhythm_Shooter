using System.Collections.Generic;
using Core.Behaviour.SingletonBehaviour;
using UnityEngine;

namespace Core.Game.VisualFX {
    public class VFXManager : SingletonBase<VFXManager> {
        private Dictionary<ParticleSystem, Queue<ParticleSystem>> _registeredParticles;

        protected override void Awake() {
            base.Awake();
            Initialize();
        }

        /// <summary>
        /// Create an object pool for certain particle system
        /// </summary>
        /// <param name="particle"> Particles to be cached </param>
        /// <param name="poolCount"> Amount of particles that will be stored at object pool </param>
        public void RegisterParticles(ParticleSystem particle, int poolCount) {
            var newQueue = new Queue<ParticleSystem>(poolCount);
            
            for (var i = 0; i < poolCount; i++) {
                newQueue.Enqueue(Instantiate(particle, transform));    
            }
            
            _registeredParticles.Add(particle, newQueue);
        }

        /// <summary>
        /// Destroys all particles that were previously initialized.
        /// </summary>
        /// <param name="particle"> Type of particles to destroy </param>
        public void ClearParticles(ParticleSystem particle) {
            if (!_registeredParticles.TryGetValue(particle, out var queue)) {
                return;
            }

            foreach (var value in queue) {
                Destroy(value.gameObject);
            }
                
            queue.Clear();
            _registeredParticles.Remove(particle);
        }

        /// <summary>
        /// Destroys all particles created with this object.
        /// </summary>
        public void ClearAllParticles() {
            foreach (var queue in _registeredParticles.Values) {
                foreach (var value in queue) {
                    Destroy(value.gameObject);
                }
                
                queue.Clear();
            }
            
            _registeredParticles.Clear();
        }

        /// <summary>
        /// Plays given particles. If Particles were registered later, they will be pulled from object pool;
        /// Otherwise new instance will be created.
        /// </summary>
        /// <param name="particle"> Particles to play </param>
        /// <param name="position"> World position where to play them </param>
        public void PlayParticles(ParticleSystem particle, Vector3 position) {
            // Check for registered particles
            if (_registeredParticles.TryGetValue(particle, out var queue)) {
                // If there are inactive particles that can be used
                if (!queue.Peek().isPlaying) {
                    var newParticles = queue.Dequeue();

                    newParticles.transform.position = position;
                    newParticles.Play();

                    queue.Enqueue(newParticles);
                }
            }
            // Else: create new instance of particles
            var newInstance = Instantiate(particle, transform);
            newInstance.transform.position = position;
            
            newInstance.Play();
            Destroy(newInstance.gameObject, particle.main.duration);
        }
        
        private void Initialize() {
            _registeredParticles = new Dictionary<ParticleSystem, Queue<ParticleSystem>>();
        }
    }
}