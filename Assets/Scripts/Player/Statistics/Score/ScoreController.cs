using System;
using Interactable;
using Interactable.Damageable;
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
        private int _currentPerfectHitStreak;
        [SerializeField] int _perfectHitsToSlowRankDrain = 10;
        public int Combo => _comboCounter;
        
        private RankManager _rankManager;
        private float _rankPerformance;
        private bool _disableRankPerformanceDrain;
        private bool _slowerRankPerformanceDrain;
        [SerializeField] private float _rankPerformanceSlowFactor = 0.5f;

        private void OnEnable() {
            PlayerEvents.AttackFailed += TryBreakComboCounter;
            PlayerEvents.DamageDealt += IncreaseComboCounter;

            void UpdateOnMiss() {
                ResetPerfectHitStreak();
                ChangeRankFill(_rankManager.CurrentRank.MissDecrement);
            }
            void UpdateOnGood() {
                ResetPerfectHitStreak();
                ChangeRankFill(_rankManager.CurrentRank.GoodIncrement);
            }
            void UpdateOnPerfect() {
                ChangeRankFill(_rankManager.CurrentRank.PerfectIncrement);
                _currentPerfectHitStreak += 1;
                if (_currentPerfectHitStreak >= _perfectHitsToSlowRankDrain) 
                    _slowerRankPerformanceDrain = true;
            }
            PlayerActionEvents.MissPerformed += UpdateOnMiss;
            PlayerActionEvents.GoodPerformed += UpdateOnGood;
            PlayerActionEvents.PerfectPerformed += UpdateOnPerfect;

            _unsubscribeFromEvents = () => {
                PlayerActionEvents.MissPerformed -= UpdateOnMiss;
                PlayerActionEvents.GoodPerformed -= UpdateOnGood;
                PlayerActionEvents.PerfectPerformed -= UpdateOnPerfect;
            };
        }

        private void Awake() {
            _rankManager = new RankManager();
        }

        private void Update() {
            if (_rankPerformance > 0f && !_disableRankPerformanceDrain) {
                _rankPerformance += _slowerRankPerformanceDrain
                    ? _rankManager.CurrentRank.PassiveDecrement * Time.deltaTime * _rankPerformanceSlowFactor
                    : _rankManager.CurrentRank.PassiveDecrement * Time.deltaTime;
                if (_rankPerformance < 0f) _rankPerformance = 0f;
                PlayerEvents.OnRankPerformanceChanged(_rankPerformance);
            }
        }

        private void OnDisable() {
            _unsubscribeFromEvents();
            PlayerEvents.AttackFailed -= TryBreakComboCounter;
            PlayerEvents.DamageDealt -= IncreaseComboCounter;
        }

        public void AddScore(int score) {
            _currentScore += (int)(score * _rankManager.CurrentRank.ScoreMultiplayer);
            PlayerEvents.OnScoreChanged(_currentScore);
        }

        private void IncreaseComboCounter(DamageInfo _) {
            _comboCounter += 1;
            _comboProtector = true;
            if (_maxComboRecorded < _comboCounter) _maxComboRecorded = _comboCounter;
            PlayerEvents.OnComboCountChanged(_comboCounter);
        }
        
        private void ResetPerfectHitStreak() {
            _currentPerfectHitStreak = 0;
            _slowerRankPerformanceDrain = false;
            _disableRankPerformanceDrain = false;
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
            if (_rankPerformance + amount >= 1f) {
                if (!_rankManager.CanPromote()) {
                    _rankPerformance = 1f;
                    _disableRankPerformanceDrain = true;
                    PlayerEvents.OnRankPerformanceChanged(_rankPerformance);
                }
                else {
                    _rankPerformance += amount - 1f;
                    _rankManager.PromoteRank();
                    PlayerEvents.OnRankIncreased();
                }
            }
            else if (_rankPerformance + amount < 0) {
                if (!_rankManager.CanDemote()) {
                    _rankPerformance = 0f;
                }
                else {
                    _rankPerformance = 1 + (_rankPerformance + amount);
                    _rankManager.DemoteRank();
                    PlayerEvents.OnRankDecreased();
                }
            }
            else _rankPerformance += amount;
        }
    }
}