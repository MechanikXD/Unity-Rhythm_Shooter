using System;
using System.Collections.Generic;
using Core.Music.Songs.Scriptable_Objects;
using UI;
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

        private readonly List<Action> _onNextBeat = new List<Action>();
        private readonly List<Action> _onNextHalfBeat = new List<Action>();
        private readonly List<Action> _beforeNextBeat = new List<Action>();
        private readonly List<Action> _afterNextBeat = new List<Action>();

        private bool _afterBeatWasCalled;
        private bool _beforeBeatWasCalled;
        
        public static event Action NextBeat;
        public static event Action NextHalfBeat;
        
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

        public float FullBeatHitWindow => _goodHitWindow * 2;
        public float HalfBeatHitWindow => _goodHitWindow;

        public struct BeatHitInfo {
            public BeatHitType HitType { get; }
            public BeatHitRelative Relative { get; }
            public float Distance { get; }

            public BeatHitInfo(BeatHitType type, BeatHitRelative relative, float distance) {
                HitType = type;
                Relative = relative;
                Distance = distance;
            }
        }
        
        private void OnDisable() {
            UIManager.PauseStateEntered -= _songSource.Pause;
            UIManager.PauseStateExited -= _songSource.UnPause;
        }

        /// <summary>
        /// Initializes all necessary fields and references of this singleton and start playing tha music
        /// </summary>
        /// <param name="songData"> record class with info about the song </param>
        /// <param name="audioSource"> Source of music </param>
        public void Initialize(SongData songData, AudioSource audioSource) {
            _songData = songData;
            _songSource = audioSource;
            _disabledInteractionsCount = 0;
            
            _lastBeat = 0f;
            _songSource.clip = songData.Audio;
            _dspSongTime = (float)AudioSettings.dspTime;
            _songSource.Play();
            _isInitialized = true;
            
            UIManager.PauseStateEntered += _songSource.Pause;
            UIManager.PauseStateExited += _songSource.UnPause;
        }
        /// <summary> Set that interaction was performed in current beat </summary>
        public void SetInteractedThisBeat() => _interactedThisBeat = true;
        /// <summary>
        /// Calls given action on very next beat
        /// Do not call this method in OnDisable or OnDestroy 
        /// </summary>
        /// <param name="action"> Action that will be called </param>
        public void AddOnNextBeat(Action action) => _onNextBeat.Add(action);
        /// <summary>
        /// Calls given action on very next half beat, beats itself also counts.
        /// Do not call this method in OnDisable or OnDestroy 
        /// </summary>
        /// <param name="action"> Action that will be called </param>
        public void AddOnNextHalfBeat(Action action) => _onNextHalfBeat.Add(action);
        /// <summary>
        /// Calls given action just before players "good" action window 
        /// Do not call this method in OnDisable or OnDestroy 
        /// </summary>
        /// <param name="action"> Action that will be called </param>
        public void AddBeforeNextBeat(Action action) => _beforeNextBeat.Add(action);
        /// <summary>
        /// Calls given action just after players "good" action window has passed
        /// Do not call this method in OnDisable or OnDestroy 
        /// </summary>
        /// <param name="action"> Action that will be called </param>
        public void AddAfterNextBeat(Action action) => _afterNextBeat.Add(action);
        /// <summary>
        /// Determines "Quality" of action based on given song position.
        /// Calculated as relative distance to closest beat.  
        /// </summary>
        /// <param name="songPosition"> position of the song when action was performed </param>
        /// <param name="ignoreDisabled"> Do this action despite previous interactions </param>
        /// <returns> Said "Quality of the action" </returns>
        public BeatHitInfo GetBeatHitInfo(float songPosition, bool ignoreDisabled=false) {
            var lastBeatTime = _lastBeat;
            var nextBeatTime = _lastBeat + SongData.Crotchet;

            var distanceToLastBeat = Mathf.Abs(songPosition - lastBeatTime);
            var distanceToNextBeat = Mathf.Abs(nextBeatTime - songPosition);
            
            var relative = distanceToLastBeat < distanceToNextBeat
                ? BeatHitRelative.Late
                : BeatHitRelative.Early;
            
            var closestDistance = Mathf.Min(distanceToNextBeat, distanceToLastBeat);
            // Last beat relative (late) < next beat relative (early)

            BeatHitType hitType; 
            if (_interactionsDisabled && !ignoreDisabled) hitType = BeatHitType.Disabled;
            else if (_interactedThisBeat && !ignoreDisabled) hitType = BeatHitType.Miss;
            // Late perfect || Early perfect
            else if (closestDistance <= _perfectHitWindow) hitType = BeatHitType.Perfect;
            // Late good  || Early good
            else if (closestDistance <= _goodHitWindow) hitType = BeatHitType.Good;
            else hitType = BeatHitType.Miss;

            return new BeatHitInfo(hitType, relative, closestDistance);
        }

        public BeatHitInfo GetBeatHitInfo(bool ignoreDisabled = false) =>
            GetBeatHitInfo(SongPosition, ignoreDisabled);

        /// <summary>
        /// Disables interaction with next several beats.
        /// if disabled, <b>DetermineHitQuality</b> will return <b>BeatHitType.Disabled</b>.
        /// </summary>
        /// <param name="count"> amount of beats to disable, including current(next) one </param>
        /// <param name="halfBeats"> amount of half beats to additionally disable </param>
        public void DisableNextInteractions(int count, int halfBeats=0) {
            _interactionsDisabled = true;
            var earlyHalfBeat = GetBeatHitInfo().Relative == BeatHitRelative.Early ? 1 : 0;
            if (UIManager.Instance.Crosshair != null)
                UIManager.Instance.Crosshair.SetNextBeatsInactive(count + earlyHalfBeat, 0);
            
            // Including current beat, because counting on half crochet need to be *2
            _disabledInteractionsCount = count * 2 + halfBeats + earlyHalfBeat;
        }

        private void Update() {
            if (!_isInitialized) return;
            // Update song position
            _songPosition = (float)(AudioSettings.dspTime - _dspSongTime) * _songSource.pitch - _songData.Offset;
            _songBeatPosition = (int)(_songPosition / _songData.Crotchet);
            // Beat passed
            if (_songPosition > _lastBeat + _songData.Crotchet) {
                _interactedThisBeat = false;
                _beforeBeatWasCalled = false;
                _afterBeatWasCalled = false;
                
                NextBeat?.Invoke();
                _lastBeat += _songData.Crotchet;

                if (_onNextBeat.Count > 0) {
                    foreach (var action in _onNextBeat) action();
                    _onNextBeat.Clear();
                }
            }
            // Half beat passed
            if (_songPosition > _lastHalfBeat + _songData.HalfCrotchet) {
                NextHalfBeat?.Invoke();
                _lastHalfBeat = _songPosition;

                if (_onNextHalfBeat.Count > 0) {
                    foreach (var action in _onNextHalfBeat) action();
                    _onNextHalfBeat.Clear();
                }
                
                if (_disabledInteractionsCount > 0) {
                    _disabledInteractionsCount--;
                    if (_disabledInteractionsCount == 0) _interactionsDisabled = false;
                }
            }
            // After beat
            if (!_afterBeatWasCalled && _songPosition > _lastBeat + _goodHitWindow) {
                if (_afterNextBeat.Count > 0) {
                    foreach (var action in _afterNextBeat) action();
                    _afterNextBeat.Clear();
                }

                _afterBeatWasCalled = true;
            }
            // Before beat
            if (!_beforeBeatWasCalled && _songPosition > _lastBeat + _songData.Crotchet - _goodHitWindow) {
                if (_beforeNextBeat.Count > 0) {
                    foreach (var action in _beforeNextBeat) action();
                    _beforeNextBeat.Clear();
                }

                _beforeBeatWasCalled = true;
            }
            
            // Update loop count
            if (_loopBeatPosition >= _songData.BeatsPerLoop)
                _completedLoops++;
            _loopBeatPosition = _songBeatPosition - _completedLoops * _songData.BeatsPerLoop;
        }
    }
}