using System;
using System.Collections.Generic;
using Core.Game;
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
        
        [SerializeField] private float _perfectHitWindow = 0.11f;
        [SerializeField] private float _goodHitWindow = 0.17f;

        private Dictionary<string, Action> _onEveryBeat;
        private Dictionary<string, (int count, Action action)> _onNextBeats;
        private List<Action> _onNextBeat;
        
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

            _onEveryBeat = new Dictionary<string, Action>();
            _onNextBeat = new List<Action>();
            _onNextBeats = new Dictionary<string, (int count, Action action)>();
            
            _lastBeat = 0f;
            _songSource.clip = songData.Audio;
            _dspSongTime = (float)AudioSettings.dspTime;
            _songSource.Play();
            _isInitialized = true;
        }
        /// <summary> Set that interaction was performed in current beat </summary>
        public void SetInteractedThisBeat() => _interactedThisBeat = true;

        /// <summary>
        /// Get relative position to closest beat
        /// </summary>
        /// <param name="songPosition"> Position of the song to check </param>
        /// <returns> Late or early </returns>
        public BeatHitRelative GetRelativeType(float songPosition) {
            var lastBeatTime = _lastBeat;
            var nextBeatTime = _lastBeat + SongData.Crotchet;
            // Last beat relative (late) < next beat relative (early)
            return Mathf.Abs(songPosition - lastBeatTime) < Mathf.Abs(nextBeatTime - songPosition)
                ? BeatHitRelative.Late
                : BeatHitRelative.Early;
        }

        public BeatHitRelative GetRelativeType() => GetRelativeType(SongPosition);
        /// <summary>
        /// Register an actions that will be called each beat
        /// </summary>
        /// <param name="key"> Unique identifier for this action, so can be accessed if needed </param>
        /// <param name="action"> Action that will be called </param>
        public void AppendRepeatingAction(string key, Action action) => _onEveryBeat.Add(key, action);
        /// <summary>
        /// Remove an action that previously has been called each beat
        /// </summary>
        /// <param name="key"> Unique identifier for said action </param>
        public void RemoveRepeatingAction(string key) => _onEveryBeat.Remove(key);
        /// <summary>
        /// Calls given action on very next beat
        /// </summary>
        /// <param name="action"> Action that will be called </param>
        public void CallOnNextBeat(Action action) => _onNextBeat.Add(action);
        /// <summary>
        /// Register an action to be called to a certain duration (in beats)
        /// </summary>
        /// <param name="key"> Unique identifier for this action, so can be accessed if needed </param>
        /// <param name="action"> Action that will be called </param>
        /// <param name="callCount"> Amount of times said action will be called </param>
        public void AddContinuousAction(string key, Action action, int callCount) =>
            _onNextBeats.Add(key, (callCount, action));
        /// <summary>
        /// Remove an action that previously has been called for some duration
        /// </summary>
        /// <param name="key"> Unique identifier for said action </param>
        public void RemoveContinuousAction(string key) => _onNextBeats.Remove(key);

        /// <summary>
        /// Determines "Quality" of action based on given song position.
        /// Calculated as relative distance to closest beat.  
        /// </summary>
        /// <param name="songPosition"> position of the song when action was performed </param>
        /// <param name="ignoreDisabled"> Do this action despite previous interactions </param>
        /// <returns> Said "Quality of the action" </returns>
        public BeatHitType DetermineHitQuality(float songPosition, bool ignoreDisabled=false) {
            // TODO: Ignores disabled actions? 
            if (_interactionsDisabled && !ignoreDisabled) return BeatHitType.Disabled;
            if (_interactedThisBeat && !ignoreDisabled) return BeatHitType.Miss;
            
            var lastBeatTime = _lastBeat;
            var nextBeatTime = _lastBeat + SongData.Crotchet;

            var relativeToBeat = Mathf.Min(
                Mathf.Abs(songPosition - lastBeatTime),
                Mathf.Abs(nextBeatTime - songPosition));
            
            // Late perfect || Early perfect
            if (relativeToBeat <= _perfectHitWindow) return BeatHitType.Perfect;
            // Late good  || Early good
            if (relativeToBeat <= _goodHitWindow) return BeatHitType.Good;
                
            return BeatHitType.Miss;
        }

        public BeatHitType DetermineHitQuality(bool ignoreDisabled = false) =>
            DetermineHitQuality(SongPosition, ignoreDisabled);

        /// <summary>
        /// Disables interaction with next several beats.
        /// if disabled, <b>DetermineHitQuality</b> will return <b>BeatHitType.Disabled</b>.
        /// </summary>
        /// <param name="count"> amount of beats to disable, including current(next) one </param>
        /// <param name="halfBeats"> amount of half beats to additionally disable </param>
        public void DisableNextInteractions(int count, int halfBeats=0) {
            _interactionsDisabled = true;
            if (GameManager.Instance.PlayerCrosshair != null) 
                GameManager.Instance.PlayerCrosshair.SetNextBeatsInactive(count, 0);

            // Including current beat, because counting on half crochet need to be *2
            _disabledInteractionsCount = count * 2 + halfBeats +
                GetRelativeType() == BeatHitRelative.Early ? 1 : 0;
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

                foreach (var action in _onEveryBeat.Values) action();
                foreach (var action in _onNextBeat) action();
                _onNextBeat.Clear();

                foreach (var pair in _onNextBeats) {
                    pair.Value.action();

                    if (pair.Value.count == 1) _onNextBeats.Remove(pair.Key);
                    else _onNextBeats[pair.Key] = (pair.Value.count - 1, pair.Value.action);
                }
            }
            // Half beat passed
            if (_songPosition > _lastHalfBeat + _songData.HalfCrotchet) {
                _lastHalfBeat = _songPosition;
                _disabledInteractionsCount--;
                if (_disabledInteractionsCount <= 0) _interactionsDisabled = false;
            }
            // Update loop count
            if (_loopBeatPosition >= _songData.BeatsPerLoop)
                _completedLoops++;
            _loopBeatPosition = _songBeatPosition - _completedLoops * _songData.BeatsPerLoop;
        }
    }
}