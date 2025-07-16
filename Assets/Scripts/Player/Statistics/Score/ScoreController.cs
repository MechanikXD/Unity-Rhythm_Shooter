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

        private void OnEnable() {
            PlayerEvents.AttackFailed += TryBreakComboCounter;
            PlayerEvents.DamageDealt += IncreaseComboCounter;

            void ChangeRankFillOnMiss() => ChangeRankFill(_rankManager.CurrentRank.MissDecrement);
            void ChangeRankFillOnGood() => ChangeRankFill(_rankManager.CurrentRank.GoodIncrement);
            void ChangeRankFillOnPerfect() => ChangeRankFill(_rankManager.CurrentRank.PerfectIncrement);
            PlayerActionEvents.MissPerformed += ChangeRankFillOnMiss;
            PlayerActionEvents.GoodPerformed += ChangeRankFillOnGood;
            PlayerActionEvents.PerfectPerformed += ChangeRankFillOnPerfect;

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
            _rankFill += _rankManager.CurrentRank.PassiveDecrement * Time.deltaTime;
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
                    PlayerEvents.OnRankIncreased(_rankManager.CurrentRank.Letter);
                }
            }
            else if (_rankFill + amount < 0) {
                if (!_rankManager.CanDemote()) {
                    _rankFill = 0f;
                }
                else {
                    _rankFill = 1 + (_rankFill + amount);
                    _rankManager.DemoteRank();
                    PlayerEvents.OnRankDecreased(_rankManager.CurrentRank.Letter);
                }
            }
            else _rankFill += amount;
        }
    }
}