using System.Collections;
using System.Collections.Generic;
using Core.Behaviour.SingletonBehaviour;
using UnityEngine;

namespace Core.Game.Audio {
    public class SoundManager : SingletonBase<SoundManager> {
        private HashSet<AudioSource> _detachedSources;
        [SerializeField] private AudioSource _loopAudioSource;
        [SerializeField] private AudioSource _musicSource;
        
        [SerializeField] private int _localSourcePoolCount = 31;
        [SerializeField] private AudioSource _localSource;
        private Queue<AudioSource> _localSourcePool;
        
        [SerializeField] private int _globalSourcePoolCount = 7;
        [SerializeField] private AudioSource _globalAudioSource;
        private Queue<AudioSource> _globalSourcePool;
        
        protected override void Awake() {
            base.Awake();
            Initialize();
        }

        public void SetMusicVolume(float newValue) => _musicSource.volume = newValue;

        public void SetSoundVolume(float newValue) {
            foreach (var source in _detachedSources) {
                source.volume = newValue;
            }

            foreach (var source in _localSourcePool) {
                source.volume = newValue;
            }

            foreach (var source in _globalSourcePool) {
                source.volume = newValue;
            }
        }

        public void PlayMusic(AudioClip clip) {
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource.Stop();

        public void PlaySound(AudioClip clip, Vector3 position, float reach) {
            if (_localSourcePool.Peek().isPlaying) {
                var newSource = Instantiate(_localSource, transform);
                newSource.clip = clip;
                newSource.transform.position = position;
                newSource.maxDistance = reach;
                
                newSource.Play();
                _detachedSources.Add(newSource);
                StartCoroutine(DestroyDetachedSourceAfterFinish(newSource));
                return;
            }

            var source = _localSourcePool.Dequeue();

            source.clip = clip;
            source.transform.position = position;
            source.maxDistance = reach;
            
            source.Play();
            _localSourcePool.Enqueue(source);
        }

        public void PlayGlobal(AudioClip clip) {
            if (_globalSourcePool.Peek().isPlaying) {
                var newSource = Instantiate(_globalAudioSource, transform);
                newSource.clip = clip;
                
                newSource.Play();
                _detachedSources.Add(newSource);
                StartCoroutine(DestroyDetachedSourceAfterFinish(newSource));
                return;
            }

            var source = _localSourcePool.Dequeue();

            source.clip = clip;
            
            source.Play();
            _localSourcePool.Enqueue(source);
        }

        public void PlaySoundAt(AudioClip clip, Transform point, float reach) {
            var newSource = Instantiate(_localSource, point);
            newSource.clip = clip;
            newSource.maxDistance = reach;
            newSource.transform.position = Vector3.zero;
            
            newSource.Play();
            StartCoroutine(DestroyDetachedSourceAfterFinish(newSource));
        }

        public AudioSource PlayLoop(AudioClip clip, Vector3 position, float reach) {
            var newSource = Instantiate(_loopAudioSource, transform);
            newSource.clip = clip;
            newSource.transform.position = position;
            newSource.maxDistance = reach;
                
            newSource.Play();
            return newSource;
        }

        public AudioSource PlayLoopAt(Transform point, AudioClip clip, float reach) {
            var newSource = Instantiate(_loopAudioSource, point);
            newSource.clip = clip;
            newSource.transform.position = Vector3.zero;
            newSource.maxDistance = reach;
                
            newSource.Play();
            return newSource;
        }

        private IEnumerator DestroyDetachedSourceAfterFinish(AudioSource source) {
            yield return new WaitForSeconds(source.clip.length);

            _detachedSources.Remove(source);
            Destroy(source.gameObject);
        }

        private void Initialize() {
            _detachedSources = new HashSet<AudioSource>();
            
            _localSourcePool = new Queue<AudioSource>();
            for (var i = 0; i < _localSourcePoolCount; i++) {
                _localSourcePool.Enqueue(Instantiate(_localSource, transform));
            }

            _globalSourcePool = new Queue<AudioSource>();
            for (var i = 0; i < _globalSourcePoolCount; i++) {
                _globalSourcePool.Enqueue(Instantiate(_globalAudioSource, transform));
            }
        }
    }
}