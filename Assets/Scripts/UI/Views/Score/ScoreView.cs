using Player;
using TMPro;
using UnityEngine;

namespace UI.Views.Score {
    public class ScoreView : MonoBehaviour {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _comboText;
        
        [SerializeField] private RankView[] _rankObjects;
        private int _currentRankIndex;

        public void Awake() => SetDefaultValues();
        public void OnEnable() => SubscribeToEvents();
        public void OnDisable() => UnsubscribeFromEvents();

        private void SubscribeToEvents() {
            PlayerEvents.ScoreChanged += SetScore;
            PlayerEvents.ComboCountChanged += SetCombo;
            PlayerEvents.RankPerformanceChanged += SetRankPerformance;
            PlayerEvents.RankIncreased += AdvanceRank;
            PlayerEvents.RankDecreased += RegressRank;
        }

        private void UnsubscribeFromEvents() {
            PlayerEvents.ScoreChanged -= SetScore;
            PlayerEvents.ComboCountChanged -= SetCombo;
            PlayerEvents.RankPerformanceChanged -= SetRankPerformance;
            PlayerEvents.RankIncreased -= AdvanceRank;
            PlayerEvents.RankDecreased -= RegressRank;
        }

        private void SetDefaultValues() {
            _scoreText.SetText("0");
            _comboText.SetText("0");

            foreach (var rank in _rankObjects) {
                rank.ChangeFill(0);
                rank.Disable();
            }

            _currentRankIndex = 0;
            _rankObjects[_currentRankIndex].Enable();
        }
        
        private void SetScore(long newValue) => _scoreText.SetText(newValue.ToString());
        private void SetCombo(int newValue) => _comboText.SetText(newValue.ToString());

        private void SetRankPerformance(float newValue) =>
            _rankObjects[_currentRankIndex].ChangeFill(newValue);

        private void AdvanceRank() {
            _rankObjects[_currentRankIndex].ChangeFill(0);
            _rankObjects[_currentRankIndex].Disable();

            _currentRankIndex += 1;
            _rankObjects[_currentRankIndex].Enable();
        }
        private void RegressRank() {
            _rankObjects[_currentRankIndex].ChangeFill(0);
            _rankObjects[_currentRankIndex].Disable();

            _currentRankIndex -= 1;
            _rankObjects[_currentRankIndex].Enable();
        }
    }
}