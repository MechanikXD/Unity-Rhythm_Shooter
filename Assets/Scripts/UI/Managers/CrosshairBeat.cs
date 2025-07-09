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
        [SerializeField] private int maxBeatsPerSide = 5;
        private (Beat left, Beat right)[] _beatPool;
        private int _beatPoolIndex;
        
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

        private void OnEnable() {
            PlayerEvents.LeftActionEvent += ShowLeftGradient;
            PlayerEvents.RightActionEvent += ShowRightGradient;
            PlayerEvents.BothActionsEvent += ShowLeftGradient;
            PlayerEvents.BothActionsEvent += ShowRightGradient;

            ConductorEvents.BeatMissedEvent += ShowMissPopUp;
            ConductorEvents.GoodBeatHitEvent += ShowGoodPopUp;
            ConductorEvents.PerfectBeatHitEvent += ShowPerfectPopUp;
        }

        private void Start() {
            // This will skip first beat
            ConductorEvents.NextBeatEvent += SpawnBeatFromPool;

            _beatPool = new (Beat left, Beat right)[maxBeatsPerSide];
            for (var i = 0; i < maxBeatsPerSide; i++) {
                _beatPool[i] = InstantiateNewBeats();
            }
        }

        private void OnDisable() {
            PlayerEvents.LeftActionEvent -= ShowLeftGradient;
            PlayerEvents.RightActionEvent -= ShowRightGradient;
            PlayerEvents.BothActionsEvent -= ShowLeftGradient;
            PlayerEvents.BothActionsEvent -= ShowRightGradient;
            
            ConductorEvents.BeatMissedEvent -= ShowMissPopUp;
            ConductorEvents.GoodBeatHitEvent -= ShowGoodPopUp;
            ConductorEvents.PerfectBeatHitEvent -= ShowPerfectPopUp;
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

        private void ShowLeftGradient(float _) {
            if (_leftGradientAnimation is { active: true }) {
                _leftGradientAnimation.Restart();
            }
            else {
                _leftGradientAnimation = DOTween.Sequence();
                _leftGradientAnimation.Append(leftGradientImage.DOColor(Color.white,
                    fadeInOutTime.x));
                _leftGradientAnimation.Append(leftGradientImage.DOColor(_transparentWhite,
                    fadeInOutTime.y));
                _leftGradientAnimation.Play();
            }
        }

        private void ShowRightGradient(float _) {
            if (_rightGradientAnimation is { active: true }) {
                _rightGradientAnimation.Restart();
            }
            else {
                _rightGradientAnimation = DOTween.Sequence();
                _rightGradientAnimation.Append(rightGradientImage
                    .DOColor(Color.white, fadeInOutTime.x));
                _rightGradientAnimation.Append(
                    rightGradientImage.DOColor(_transparentWhite, fadeInOutTime.y));
                _rightGradientAnimation.Play();
            }
        }

        private void ShowPopUp(TextMeshProUGUI textField) {
            _currentPopUp?.Complete();
            _currentPopUp = DOTween.Sequence();
            _currentPopUp.Append(textField.DOColor(Color.black, fadeInOutTime.x));
            _currentPopUp.Append(textField.DOColor(_transparentBlack, fadeInOutTime.y));
            _currentPopUp.Play();
        }

        private void ShowPerfectPopUp() => ShowPopUp(perfectText);
        private void ShowGoodPopUp() => ShowPopUp(goodText);
        private void ShowMissPopUp() => ShowPopUp(missText);

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