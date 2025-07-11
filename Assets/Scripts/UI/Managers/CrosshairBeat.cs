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
        // TODO: Fix incorrect beat spawning/speed
        [SerializeField] private Image beatPrefab;
        [SerializeField] private int maxBeatsPerSide = 5;
        private (Beat left, Beat right)[] _beatPool;
        private int _beatPoolIndex;
        private Action _unsubscribeFromEventsAction;
        
        [SerializeField] private BeatSettings beatSettings;

        [SerializeField] private Image leftGradientImage;
        [SerializeField] private Image rightGradientImage;
        [SerializeField] private Vector2 fadeInOutTime = new Vector2(0.05f, 0.5f);
        private Sequence _leftGradientAnimation;
        private Sequence _rightGradientAnimation;

        [SerializeField] private RectTransform leftBeatArea;
        [SerializeField] private RectTransform rightBeatArea;
        
        [SerializeField] private float singleBeatTime = 1.1f;

        [SerializeField] private TextMeshProUGUI perfectText;
        [SerializeField] private TextMeshProUGUI goodText;
        [SerializeField] private TextMeshProUGUI missText;
        private Sequence _currentPopUp;

        private readonly Color _transparentWhite = new Color(255, 255, 255, 0);
        private readonly Color _transparentBlack = new Color(0, 0, 0, 0);

        private void OnEnable() => SubscribeToEvents();

        private void Start() {
            _beatPool = new (Beat left, Beat right)[maxBeatsPerSide];
            for (var i = 0; i < maxBeatsPerSide; i++) {
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
            PlayerActionEvents.PerfectPerformed += ShowPerfectPopUp;
            PlayerActionEvents.GoodPerformed += ShowGoodPopUp;
            PlayerActionEvents.MissPerformed += ShowMissPopUp;

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
            };
            
            // This will skip first beat
            ConductorEvents.NextBeatEvent += SpawnBeatFromPool;
        }
        
        private void UnsubscribeFromEvents() {
            _unsubscribeFromEventsAction?.Invoke();
            _unsubscribeFromEventsAction = null;    // Clean up

            ConductorEvents.NextBeatEvent -= SpawnBeatFromPool;
        }

        private (Beat leftBeat, Beat rightBeat) InstantiateNewBeats() {
            var left = new Beat();
            left.InstantiateSelf(beatPrefab, leftBeatArea, false);
            left.SetDefaultState(beatSettings);

            var right = new Beat();
            right.InstantiateSelf(beatPrefab, rightBeatArea, true);
            right.SetDefaultState(beatSettings);

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
            _beatPool[index].left.Animate(beatSettings, singleBeatTime);
            _beatPool[index].right.Animate(beatSettings, singleBeatTime);
        }
    }
}