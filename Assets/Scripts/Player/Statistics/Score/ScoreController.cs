using System;
using Player.Statistics.Score.Rank;
using UnityEngine;

namespace Player.Statistics.Score {
    public class ScoreController : MonoBehaviour {
        private static ScoreController _instance;
        private Action _unsubscribeFromEvents;

        public static ScoreController Instance {
            get {
                if (_instance != null) return _instance;

                var newGameObj = new GameObject(nameof(ScoreController));
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance = newGameObj.AddComponent<ScoreController>();

                return _instance;
            }
        }
        
        private long _currentScore;
        public long Score => _currentScore;
        
        private int _comboCounter;
        private bool _comboProtector = true;
        private int _maxComboRecorded;
        public int Combo => _comboCounter;
        
        private RankManager _rankManager;
        // TODO: Change this to meaningful names
        private float _rankFill;
        
        // TODO: Remove after testing
        private RankInfo _currentRank;
        private void UpdateCurrentRank() => _currentRank = _rankManager.CurrentRank;

        private void OnEnable() {
            PlayerEvents.AttackFailed += TryBreakComboCounter;
            PlayerEvents.DamageDealt += IncreaseComboCounter;

            void ChangeRankFillOnMiss() => ChangeRankFill(_rankManager.CurrentRank.MissDecrement);
            void ChangeRankFillOnGood() => ChangeRankFill(_rankManager.CurrentRank.GoodIncrement);
            void ChangeRankFillOnPerfect() => ChangeRankFill(_rankManager.CurrentRank.PerfectIncrement);
            PlayerActionEvents.MissPerformed += ChangeRankFillOnMiss;
            PlayerActionEvents.GoodPerformed += ChangeRankFillOnGood;
            PlayerActionEvents.PerfectPerformed += ChangeRankFillOnPerfect;
            // TODO: Remove after testing
            PlayerEvents.RankDecreased += UpdateCurrentRank;
            PlayerEvents.RankIncreased += UpdateCurrentRank;

            _unsubscribeFromEvents = () => {
                PlayerActionEvents.MissPerformed -= ChangeRankFillOnMiss;
                PlayerActionEvents.GoodPerformed -= ChangeRankFillOnGood;
                PlayerActionEvents.PerfectPerformed -= ChangeRankFillOnPerfect;
            };
        }

        private void Awake() {
            _rankManager = new RankManager();
        }

        private void Update() {
            if (_rankFill > 0f) {
                _rankFill += _rankManager.CurrentRank.PassiveDecrement * Time.deltaTime;
                if (_rankFill < 0f) _rankFill = 0f;
            }
        }

        private void OnDisable() {
            _unsubscribeFromEvents();
            PlayerEvents.AttackFailed -= TryBreakComboCounter;
            PlayerEvents.DamageDealt -= IncreaseComboCounter;
            // TODO: Remove after testing
            PlayerEvents.RankDecreased -= UpdateCurrentRank;
            PlayerEvents.RankIncreased -= UpdateCurrentRank;
        }

        public void AddScore(int score) {
            _currentScore += (int)(score * _rankManager.CurrentRank.ScoreMultiplayer);
            PlayerEvents.OnScoreChanged(_currentScore);
        }

        private void IncreaseComboCounter() {
            _comboCounter += 1;
            _comboProtector = true;
            if (_maxComboRecorded < _comboCounter) _maxComboRecorded = _comboCounter;
            PlayerEvents.OnComboCountChanged(_comboCounter);
        }

        private void TryBreakComboCounter() {
            if (_comboProtector) {
                _comboProtector = false;
            }
            else {
                _comboCounter = 0;
                PlayerEvents.OnComboCountChanged(_comboCounter);
            }
        }

        private void ChangeRankFill(float amount) {
            if (_rankFill + amount >= 1f) {
                if (!_rankManager.CanPromote()) {
                    _rankFill = 1f;
                }
                else {
                    _rankFill += amount - 1f;
                    _rankManager.PromoteRank();
                    PlayerEvents.OnRankIncreased();
                }
            }
            else if (_rankFill + amount < 0) {
                if (!_rankManager.CanDemote()) {
                    _rankFill = 0f;
                }
                else {
                    _rankFill = 1 + (_rankFill + amount);
                    _rankManager.DemoteRank();
                    PlayerEvents.OnRankDecreased();
                }
            }
            else _rankFill += amount;
        }
    }
}