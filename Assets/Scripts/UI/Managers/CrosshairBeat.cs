using System;
using Core.Music;
using DG.Tweening;
using Player;
using TMPro;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class CrosshairBeat : MonoBehaviour {
        [SerializeField] private Image beatPrefab;
        private (Beat left, Beat right)[] _beatPool;
        private int _beatPoolIndex;
        private Action _unsubscribeFromEventsAction;
        
        [SerializeField] private BeatSettings defaultBeatSettings;
        [SerializeField] private BeatSettings missBeatSettings;

        [SerializeField] private Image leftGradientImage;
        [SerializeField] private Image rightGradientImage;
        [SerializeField] private Vector2 fadeInOutTime = new Vector2(0.05f, 0.5f);
        private Sequence _leftGradientAnimation;
        private Sequence _rightGradientAnimation;

        [SerializeField] private RectTransform leftBeatArea;
        [SerializeField] private RectTransform rightBeatArea;

        [SerializeField] private int beatsPerSide = 2;
        [SerializeField] private float beatOffset = -0.02f;
        private float _singleBeatTime;
        private int _maxBeatsPerSide;

        [SerializeField] private TextMeshProUGUI perfectText;
        [SerializeField] private TextMeshProUGUI goodText;
        [SerializeField] private TextMeshProUGUI missText;
        private Sequence _currentPopUp;

        private readonly Color _transparentWhite = new Color(255, 255, 255, 0);
        private readonly Color _transparentBlack = new Color(0, 0, 0, 0);

        private void OnEnable() => SubscribeToEvents();

        private void Start() {
            _singleBeatTime = Conductor.Instance.SongData.Crotchet * beatsPerSide;
            _maxBeatsPerSide = beatsPerSide + 1;
            
            _beatPool = new (Beat left, Beat right)[_maxBeatsPerSide];
            for (var i = 0; i < _maxBeatsPerSide; i++) {
                _beatPool[i] = InstantiateNewBeats();
            }
        }

        private void OnDisable() => UnsubscribeFromEvents();

        private void SubscribeToEvents() {
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

            void ShowPerfectPopUp() => ShowPopUp(perfectText);
            void ShowGoodPopUp() => ShowPopUp(goodText);
            void ShowMissPopUp() => ShowPopUp(missText);
            void DimMissedBeats() => ModifyBeat(0, beat => beat.SetSettings(missBeatSettings), 2);
            PlayerActionEvents.PerfectPerformed += ShowPerfectPopUp;
            PlayerActionEvents.GoodPerformed += ShowGoodPopUp;
            PlayerActionEvents.MissPerformed += ShowMissPopUp;
            PlayerActionEvents.MissPerformed += DimMissedBeats;

            _unsubscribeFromEventsAction = () => {
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
                PlayerActionEvents.MissPerformed -= DimMissedBeats;
            };
            
            // This will skip first beat
            ConductorEvents.NextBeatEvent += SpawnBeatFromPool;
        }
        
        private void UnsubscribeFromEvents() {
            _unsubscribeFromEventsAction?.Invoke();
            _unsubscribeFromEventsAction = null;    // Clean up

            ConductorEvents.NextBeatEvent -= SpawnBeatFromPool;
        }

        private void ModifyBeat(int beatIndexInInteractionOrder, Action<Beat> modifier, int count=1) {
            // Get correct index from pool (current one - number of beats per area)
            if (count + beatIndexInInteractionOrder > _maxBeatsPerSide)
                throw new IndexOutOfRangeException(
                    $"Index of beat to modify is out of range: can be max of {_maxBeatsPerSide}, given: {count + beatIndexInInteractionOrder}");
            var index = _beatPoolIndex - count - beatIndexInInteractionOrder;
            if (index < 0) index = _maxBeatsPerSide + index;

            // apply modification
            while (count > 0) {
                modifier(_beatPool[index].left);
                modifier(_beatPool[index].right);

                index++;
                if (index >= _maxBeatsPerSide) index = 0;

                count--;
            }
        }

        private (Beat leftBeat, Beat rightBeat) InstantiateNewBeats() {
            var left = new Beat();
            left.InstantiateSelf(beatPrefab, leftBeatArea, false);
            left.SetDefaultState(defaultBeatSettings);

            var right = new Beat();
            right.InstantiateSelf(beatPrefab, rightBeatArea, true);
            right.SetDefaultState(defaultBeatSettings);

            return (left, right);
        }

        private void ShowLeftGradient(float strength) {
            if (_leftGradientAnimation is { active: true }) {
                _leftGradientAnimation.Complete();
            }

            var startColor = new Color(255,255,255, strength);
            _leftGradientAnimation = DOTween.Sequence();
            _leftGradientAnimation.Append(leftGradientImage.DOColor(startColor, fadeInOutTime.x));
            _leftGradientAnimation.Append(leftGradientImage.DOColor(_transparentWhite, fadeInOutTime.y));
            _leftGradientAnimation.Play();
        }

        private void ShowRightGradient(float strength) {
            if (_rightGradientAnimation is { active: true }) {
                _rightGradientAnimation.Complete();
            }
            
            var startColor = new Color(255,255,255, strength);
            _rightGradientAnimation = DOTween.Sequence();
            _rightGradientAnimation.Append(rightGradientImage.DOColor(startColor, fadeInOutTime.x));
            _rightGradientAnimation.Append(rightGradientImage.DOColor(_transparentWhite, fadeInOutTime.y));
            _rightGradientAnimation.Play();
        }

        private void ShowPopUp(TextMeshProUGUI textField) {
            _currentPopUp?.Complete();
            _currentPopUp = DOTween.Sequence();
            _currentPopUp.Append(textField.DOColor(Color.black, fadeInOutTime.x));
            _currentPopUp.Append(textField.DOColor(_transparentBlack, fadeInOutTime.y));
            _currentPopUp.Play();
        }

        private void SpawnBeatFromPool() {
            Spawn(_beatPoolIndex);

            _beatPoolIndex++;
            if (_beatPoolIndex >= _beatPool.Length) _beatPoolIndex = 0;
        }

        private void Spawn(int index) {
            _beatPool[index].left.Animate(defaultBeatSettings, _singleBeatTime + beatOffset);
            _beatPool[index].right.Animate(defaultBeatSettings, _singleBeatTime + beatOffset);
        }
    }
}