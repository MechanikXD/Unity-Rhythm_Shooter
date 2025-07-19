using System;
using Core.Music.Songs.Scriptable_Objects;
using UnityEngine;

namespace Core.Music {
    /// <summary>
    /// Class that manages music interactions.
    /// <b>Initialize</b> method mush be called to start music playing.
    /// </summary>
    public class Conductor : MonoBehaviour {
        private static Conductor _instance;
        private bool _isInitialized;
        private bool _interactedThisBeat;
        
        [SerializeField] private float perfectHitWindow = 0.11f;
        [SerializeField] private float goodHitWindow = 0.17f;
        
        public static event Action NextBeatEvent;
        public static Conductor Instance {
            get {
                if (_instance != null) return _instance;
                
                // This is called only once per scene
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
        
        /// <summary> Data about the song that is currently playing </summary>
        public SongData SongData => _songData;
        /// <summary> How long the song has been playing </summary>
        public float SongPosition => _songPosition;
        /// <summary> How many beats have passed </summary>
        public int BeatPosition => _songBeatPosition;
        /// <summary> Position on last recorded beat </summary>
        public float LastBeat => _lastBeat;
        
        /// <summary>
        /// Initializes all necessary fields and references of this singleton and start playing tha music
        /// </summary>
        /// <param name="songData"> record class with info about the song </param>
        /// <param name="audioSource"> Source of music </param>
        public void Initialize(SongData songData, AudioSource audioSource) {
            _songData = songData;
            _songSource = audioSource;
            
            _lastBeat = 0f;
            _songSource.clip = songData.Audio;
            _dspSongTime = (float)AudioSettings.dspTime;
            _songSource.Play();
            _isInitialized = true;
        }
        /// <summary> Set that interaction was performed in current beat </summary>
        public void SetInteractedThisBeat() => _interactedThisBeat = true;
        /// <summary>
        /// Determines "Quality" of action based on given song position.
        /// Calculated as relative distance to closest beat.  
        /// </summary>
        /// <param name="songPosition"> position of the song when action was performed </param>
        /// <returns> Sad "Quality of the action" </returns>
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
        /// <summary>
        /// Disables interaction with next several beats.
        /// if disabled, <b>DetermineHitQuality</b> will return <b>BeatHitType.Disabled</b>.
        /// </summary>
        /// <param name="count"> amount of beats to disable, including current(next) one </param>
        public void DisableNextInteractions(int count) {
            _interactionsDisabled = true;
            // Including current beat, because counting on half crochet need to be *2
            _disabledInteractionsCount = count * 2;
        }
        /// <summary>
        /// Calls given function repeatedly each beat for given amount on times.
        /// </summary>
        /// <param name="function"> Function that will be called each beat </param>
        /// <param name="callCount"> Amount of calls </param>
        public static void CallOnNextBeats(Action function, int callCount) {
            void RecursiveFunctionCall() {
                function();
                callCount--;
                if (callCount == 0) NextBeatEvent -= RecursiveFunctionCall;
            }
            NextBeatEvent += RecursiveFunctionCall;
        }

        private void Update() {
            if (!_isInitialized) return;
            // Update song position
            _songPosition = (float)(AudioSettings.dspTime - _dspSongTime) * _songSource.pitch - _songData.Offset;
            _songBeatPosition = (int)(_songPosition / _songData.Crotchet);
            // Beat passed
            if (_songPosition > _lastBeat + _songData.Crotchet) {
                _interactedThisBeat = false;
                NextBeatEvent?.Invoke();
                _lastBeat += _songData.Crotchet;
            }
            // Half beat passed
            if (_songPosition > _lastHalfBeat + _songData.HalfCrotchet && _interactionsDisabled) {
                _lastHalfBeat = _songPosition;
                _disabledInteractionsCount--;
                if (_disabledInteractionsCount == 0) _interactionsDisabled = false;
            }
            // Update loop count
            if (_loopBeatPosition >= _songData.BeatsPerLoop)
                _completedLoops++;
            _loopBeatPosition = _songBeatPosition - _completedLoops * _songData.BeatsPerLoop;
        }
    }
}