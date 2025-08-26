using System;
using System.Linq;
using Core.Music;
using DG.Tweening;
using Player;
using TMPro;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class CrosshairBeat : MonoBehaviour {
        [SerializeField] private Image _beatPrefab;
        private BeatQueue _beatQueue;
        private Action _unsubscribeFromEventsAction;
        
        [SerializeField] private BeatSettings _defaultBeatSettings;
        [SerializeField] private BeatSettings _missBeatSettings;

        [SerializeField] private Image _leftGradientImage;
        [SerializeField] private Image _rightGradientImage;
        [SerializeField] private Vector2 _fadeInOutTime = new Vector2(0.05f, 0.5f);
        private Sequence _leftGradientAnimation;
        private Sequence _rightGradientAnimation;

        [SerializeField] private RectTransform _leftBeatArea;
        [SerializeField] private RectTransform _rightBeatArea;

        [SerializeField] private int _beatsPerSide = 2;
        [SerializeField] private float _beatOffset = -0.02f;
        private float _singleBeatTime;
        private int _maxBeatsPerSide;

        [SerializeField] private TextMeshProUGUI _perfectText;
        [SerializeField] private TextMeshProUGUI _goodText;
        [SerializeField] private TextMeshProUGUI _missText;
        private Sequence _currentPopUp;

        private readonly Color _transparentWhite = new Color(255, 255, 255, 0);
        private readonly Color _transparentBlack = new Color(0, 0, 0, 0);

        private void OnEnable() => SubscribeToEvents();

        private void Start() {
            _singleBeatTime = Conductor.Instance.SongData.Crotchet * _beatsPerSide;
            _maxBeatsPerSide = _beatsPerSide + 1;
            
            _beatQueue = 
                new BeatQueue(_beatPrefab, _maxBeatsPerSide, _leftBeatArea, _rightBeatArea, _defaultBeatSettings);
        }

        private void OnDisable() => UnsubscribeFromEvents();

        private void SubscribeToEvents() {
            void RunNewBeats() => _beatQueue.StartNewBeats(_singleBeatTime, _beatOffset);
            Conductor.NextBeat += RunNewBeats;
            
            void LeftPerfect() => ShowLeftGradient(1);
            void LeftGood() => ShowLeftGradient(0.65f);
            void LeftMissed() => ShowLeftGradient(0.3f);
            PlayerActionEvents.LeftPerfectPerformed += LeftPerfect;
            PlayerActionEvents.LeftGoodPerformed += LeftGood;
            PlayerActionEvents.LeftMissPerformed += LeftMissed;
            
            void RightPerfect() => ShowRightGradient(1);
            void RightGood() => ShowRightGradient(0.65f);
            void RightMissed() => ShowRightGradient(0.3f);
            PlayerActionEvents.RightPerfectPerformed += RightPerfect;
            PlayerActionEvents.RightGoodPerformed += RightGood;
            PlayerActionEvents.RightMissPerformed += RightMissed;
            
            void BothPerfect() { LeftPerfect(); RightPerfect(); }
            void BothGood() { LeftGood(); RightGood(); }
            void BothMissed() { LeftMissed(); RightMissed(); }
            PlayerActionEvents.BothPerfectPerformed += BothPerfect;
            PlayerActionEvents.BothGoodPerformed += BothGood;
            PlayerActionEvents.BothMissPerformed += BothMissed;

            void ShowPerfectPopUp() => ShowPopUp(_perfectText);
            void ShowGoodPopUp() => ShowPopUp(_goodText);
            void ShowMissPopUp() => ShowPopUp(_missText);
            PlayerActionEvents.PerfectPerformed += ShowPerfectPopUp;
            PlayerActionEvents.GoodPerformed += ShowGoodPopUp;
            PlayerActionEvents.MissPerformed += ShowMissPopUp;

            _unsubscribeFromEventsAction = () => {
                Conductor.NextBeat -= RunNewBeats;
                
                PlayerActionEvents.LeftPerfectPerformed -= LeftPerfect;
                PlayerActionEvents.LeftGoodPerformed -= LeftGood;
                PlayerActionEvents.LeftMissPerformed -= LeftMissed;

                PlayerActionEvents.RightPerfectPerformed -= RightPerfect;
                PlayerActionEvents.RightGoodPerformed -= RightGood;
                PlayerActionEvents.RightMissPerformed -= RightMissed;

                PlayerActionEvents.BothPerfectPerformed -= BothPerfect;
                PlayerActionEvents.BothGoodPerformed -= BothGood;
                PlayerActionEvents.BothMissPerformed -= BothMissed;

                PlayerActionEvents.PerfectPerformed -= ShowPerfectPopUp;
                PlayerActionEvents.GoodPerformed -= ShowGoodPopUp;
                PlayerActionEvents.MissPerformed -= ShowMissPopUp;
            };
        }
        private void UnsubscribeFromEvents() {
            _unsubscribeFromEventsAction?.Invoke();
            _unsubscribeFromEventsAction = null;    // Clean up
            _beatQueue.UnsubscribeFromEvents();
        }

        public void SetNextBeatsInactive(int count, int skipBeats) {
            ModifyBeat(skipBeats, beat => beat.SetNewSettings(_missBeatSettings), count);
        }
        /// <summary>
        /// Modifies beat playing on screen
        /// </summary>
        /// <param name="skipBeats"> How many beats to skip starting from closest one </param>
        /// <param name="modifier"> Function to modify beats </param>
        /// <param name="count"> amount of beats to be modified </param>
        private void ModifyBeat(int skipBeats, Action<Beat> modifier, int count=1) {
            // function that will modify beats and return true or false
            // Whether all actions were done or not
            var counter = count;
            bool Modify((Beat left, Beat right) beats) {
                if (skipBeats > 0) {
                    skipBeats--;
                    return false;
                }
                else {
                    counter--;
                    modifier(beats.left);
                    modifier(beats.right);
                    return counter == 0;
                }
            }
            // Modify active beats
            if (_beatQueue.Any(Modify)) return;
            // Still to few, call recursively when new inactive appears
            // TODO: Fix modifications of beats past max capacity
            void ModifyWrapper() {
                if (Modify(_beatQueue.PeekFront())) {
                    Conductor.NextBeat -= ModifyWrapper;
                }
            }
            Conductor.NextBeat += ModifyWrapper;
        }
        
        // Shows left gradient of crosshair
        private void ShowLeftGradient(float strength) {
            if (_leftGradientAnimation is { active: true }) {
                _leftGradientAnimation.Complete();
            }

            var startColor = new Color(255,255,255, strength);
            _leftGradientAnimation = DOTween.Sequence();
            _leftGradientAnimation.Append(_leftGradientImage.DOColor(startColor, _fadeInOutTime.x));
            _leftGradientAnimation.Append(_leftGradientImage.DOColor(_transparentWhite, _fadeInOutTime.y));
            _leftGradientAnimation.Play();
        }
        // Shows right gradient of crosshair
        private void ShowRightGradient(float strength) {
            if (_rightGradientAnimation is { active: true }) {
                _rightGradientAnimation.Complete();
            }
            
            var startColor = new Color(255,255,255, strength);
            _rightGradientAnimation = DOTween.Sequence();
            _rightGradientAnimation.Append(_rightGradientImage.DOColor(startColor, _fadeInOutTime.x));
            _rightGradientAnimation.Append(_rightGradientImage.DOColor(_transparentWhite, _fadeInOutTime.y));
            _rightGradientAnimation.Play();
        }
        // Shows pop-up under crosshair of action "Quality" 
        private void ShowPopUp(TextMeshProUGUI textField) {
            _currentPopUp?.Complete();
            _currentPopUp = DOTween.Sequence();
            _currentPopUp.Append(textField.DOColor(Color.black, _fadeInOutTime.x));
            _currentPopUp.Append(textField.DOColor(_transparentBlack, _fadeInOutTime.y));
            _currentPopUp.Play();
        }
    }
}