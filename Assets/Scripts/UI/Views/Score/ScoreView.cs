using Player;
using TMPro;
using UnityEngine;

namespace UI.Views.Score {
    public class ScoreView : MonoBehaviour {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        
        [SerializeField] private RankView[] rankObjects;
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
            scoreText.SetText("0");
            comboText.SetText("0");

            foreach (var rank in rankObjects) {
                rank.ChangeFill(0);
                rank.Disable();
            }

            _currentRankIndex = 0;
            rankObjects[_currentRankIndex].Enable();
        }
        
        private void SetScore(long newValue) => scoreText.SetText(newValue.ToString());
        private void SetCombo(int newValue) => comboText.SetText(newValue.ToString());

        private void SetRankPerformance(float newValue) =>
            rankObjects[_currentRankIndex].ChangeFill(newValue);

        private void AdvanceRank() {
            rankObjects[_currentRankIndex].ChangeFill(0);
            rankObjects[_currentRankIndex].Disable();

            _currentRankIndex += 1;
            rankObjects[_currentRankIndex].Enable();
        }
        private void RegressRank() {
            rankObjects[_currentRankIndex].ChangeFill(0);
            rankObjects[_currentRankIndex].Disable();

            _currentRankIndex -= 1;
            rankObjects[_currentRankIndex].Enable();
        }
    }
}