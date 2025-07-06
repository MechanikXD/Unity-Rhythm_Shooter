using Player;
using UnityEngine;

namespace Core.Music {
    public class Conductor : MonoBehaviour {
        private static Conductor _instance;
        private bool _isInitialized;
        
        [SerializeField] private float perfectHitWindow = 0.13f;
        [SerializeField] private float goodHitWindow = 0.18f;

        public static Conductor Instance {
            get {
                if (_instance != null) return _instance;

                var newObject = new GameObject(nameof(Conductor));
                _instance = newObject.AddComponent<Conductor>();
                return _instance;
            }
        }
        
        private SongData _songData;
        private AudioSource _songSource;
        
        private float _songPosition;
        private int _songBeatPosition;
        private int _loopBeatPosition;
        
        private float _dspSongTime;
        private float _lastBeat;
        private int _completedLoops;

        public SongData SongData => _songData;
        public float SongPosition => _songPosition;
        public int BeatPosition => _songBeatPosition;
        public float LastBeat => _lastBeat;

        private void OnEnable() {
            PlayerEvents.LeftActionEvent += DetermineHitQuality;
            PlayerEvents.RightActionEvent += DetermineHitQuality;
            PlayerEvents.BothActionsEvent += DetermineHitQuality;
        }

        public void Initialize(SongData songData, AudioSource audioSource) {
            _songData = songData;
            _songSource = audioSource;
            
            _lastBeat = 0f;
            _songSource.clip = songData.Audio;
            songData.Crotchet = 60f / songData.Bpm;
            _dspSongTime = (float)AudioSettings.dspTime;
            _songSource.Play();
            _isInitialized = true;
        }
        
        private void OnDisable() {
            PlayerEvents.LeftActionEvent -= DetermineHitQuality;
            PlayerEvents.RightActionEvent -= DetermineHitQuality;
            PlayerEvents.BothActionsEvent -= DetermineHitQuality;
        }

        private void DetermineHitQuality(float songPosition) {
            var lastBeatTime = _lastBeat;
            var nextBeatTime = _lastBeat + SongData.Crotchet;

            var relativeToBeat = Mathf.Min(
                Mathf.Abs(songPosition - lastBeatTime),
                Mathf.Abs(nextBeatTime - songPosition));
            // Late perfect || Early perfect
            if (relativeToBeat <= perfectHitWindow) ConductorEvents.OnPerfectBeatHit();
            // Late good  || Early good
            else if (relativeToBeat <= goodHitWindow) ConductorEvents.OnGoodBeatHit();
            else ConductorEvents.OnBeatMissed();
        }

        private void Update() {
            if (!_isInitialized) return;
            
            _songPosition = (float)(AudioSettings.dspTime - _dspSongTime) * _songSource.pitch - _songData.Offset;
            _songBeatPosition = (int)(_songPosition / _songData.Crotchet);
            
            if (_songPosition > _lastBeat + _songData.Crotchet) {
                ConductorEvents.OnOnNextBeat();
                _lastBeat += _songData.Crotchet;
            }

            if (_loopBeatPosition >= _songData.BeatsPerLoop)
                _completedLoops++;
            _loopBeatPosition = _songBeatPosition - _completedLoops * _songData.BeatsPerLoop;
        }
    }
}