using UnityEngine;

namespace Core.Music {
    public class Conductor : MonoBehaviour {
        private static Conductor _instance;
        private bool _isInitialized;
        private bool _interactedThisBeat;
        
        [SerializeField] private float perfectHitWindow = 0.11f;
        [SerializeField] private float goodHitWindow = 0.2f;

        public static Conductor Instance {
            get {
                if (_instance != null) return _instance;

                // ReSharper disable Unity.PerformanceCriticalCodeInvocation
                var newObject = new GameObject(nameof(Conductor));
                _instance = newObject.AddComponent<Conductor>();
                return _instance;
                // ReSharper restore Unity.PerformanceCriticalCodeInvocation
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
        private float _lastHalfBeat;

        private bool _interactionsDisabled;
        private int _disabledInteractionsCount;

        public SongData SongData => _songData;
        public float SongPosition => _songPosition;
        public int BeatPosition => _songBeatPosition;
        public float LastBeat => _lastBeat;

        public void Initialize(SongData songData, AudioSource audioSource) {
            _songData = songData;
            _songSource = audioSource;
            
            _lastBeat = 0f;
            _songSource.clip = songData.Audio;
            _dspSongTime = (float)AudioSettings.dspTime;
            ConductorEvents.NextBeatEvent += SetNotInteractedThisBeat;
            _songSource.Play();
            _isInitialized = true;
        }

        private void SetNotInteractedThisBeat() => _interactedThisBeat = false;
        public void SetInteractedThisBeat() => _interactedThisBeat = true;
        
        public BeatHitType DetermineHitQuality(float songPosition) {
            if (_interactionsDisabled) return BeatHitType.Disabled;
            if (_interactedThisBeat) return BeatHitType.Miss;
            
            var lastBeatTime = _lastBeat;
            var nextBeatTime = _lastBeat + SongData.Crotchet;

            var relativeToBeat = Mathf.Min(
                Mathf.Abs(songPosition - lastBeatTime),
                Mathf.Abs(nextBeatTime - songPosition));
            
            // Late perfect || Early perfect
            if (relativeToBeat <= perfectHitWindow) return BeatHitType.Perfect;
            // Late good  || Early good
            if (relativeToBeat <= goodHitWindow) return BeatHitType.Good;
                
            return BeatHitType.Miss;
        }

        public void DisableNextInteractions(int count) {
            _interactionsDisabled = true;
            // Including current beat, because counting on half crochet need to be *2
            _disabledInteractionsCount = count * 2;
        }

        private void Update() {
            if (!_isInitialized) return;
            
            _songPosition = (float)(AudioSettings.dspTime - _dspSongTime) * _songSource.pitch - _songData.Offset;
            _songBeatPosition = (int)(_songPosition / _songData.Crotchet);
            // Beat passed
            if (_songPosition > _lastBeat + _songData.Crotchet) {
                ConductorEvents.OnOnNextBeat();
                _lastBeat += _songData.Crotchet;
            }
            // Half beat passed
            if (_songPosition > _lastHalfBeat + _songData.HalfCrotchet && _interactionsDisabled) {
                _lastHalfBeat = _songPosition;
                _disabledInteractionsCount--;
                if (_disabledInteractionsCount == 0) _interactionsDisabled = false;
            }

            if (_loopBeatPosition >= _songData.BeatsPerLoop)
                _completedLoops++;
            _loopBeatPosition = _songBeatPosition - _completedLoops * _songData.BeatsPerLoop;
        }
    }
}