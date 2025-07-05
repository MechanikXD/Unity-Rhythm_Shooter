using UnityEngine;

namespace Core.Music {
    public class Conductor : MonoBehaviour {
        private static Conductor _instance;
        private bool _isInitialized;

        public static Conductor Instance {
            get {
                if (_instance != null) return _instance;

                var newObject = new GameObject(nameof(Conductor));
                _instance = newObject.AddComponent<Conductor>();
                return _instance;
            }
        }
        
        [SerializeField] private SongData songData;
        [SerializeField] private AudioSource songSource;
        private float _songPosition;
        private float _songBeatPosition;
        private float _dspSongTime;
        private float _lastBeat;

        public SongData SongData => songData;
        public float SongPosition => _songPosition;
        public float BeatPosition => _songBeatPosition;
        public float LastBeat => _lastBeat;

        public void Initialize() {
            _lastBeat = 0f;
            songSource.clip = songData.Audio;
            songData.Crotchet = 60f / songData.Bpm;
            _dspSongTime = (float)AudioSettings.dspTime;
            songSource.Play();
            _isInitialized = true;
        }

        private void Update() {
            if (!_isInitialized) return;
            
            _songPosition = (float)(AudioSettings.dspTime - _dspSongTime) * songSource.pitch - songData.Offset;
            _songBeatPosition = _songPosition / songData.Crotchet;
            
            if (_songPosition > _lastBeat + songData.Crotchet) {
                ConductorEvents.OnOnNextBeat();
                _lastBeat += songData.Crotchet;
            }
        }
    }
}