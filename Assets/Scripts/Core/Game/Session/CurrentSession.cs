using System;
using System.Collections.Generic;
using Enemy;
using Player;
using Player.Statistics.Score;

namespace Core.Game.Session {
    public static class CurrentSession {
        private static int _enemiesDefeated;
        private static List<long> _scores;
        private static List<int> _maxCombos;
        private static int _currentMaxCombo;
        private static int _currencyGained;

        private static Action _unsubscribeFromEvents;

        public static void StartNewSession() {
            _enemiesDefeated = 0;
            _scores = new List<long>();
            _maxCombos = new List<int>();
            _currencyGained = 0;
            _currentMaxCombo = 0;

            void IncreaseEnemiesDefeatedCount(EnemyDefeatedInfo _) => _enemiesDefeated++;

            void RecordMaxCombo(int value) {
                if (value > _currentMaxCombo) _currentMaxCombo = value;
            }

            EnemyEvents.EnemyDefeated += IncreaseEnemiesDefeatedCount;
            PlayerEvents.ComboCountChanged += RecordMaxCombo;

            _unsubscribeFromEvents = () => {
                EnemyEvents.EnemyDefeated -= IncreaseEnemiesDefeatedCount;
                PlayerEvents.ComboCountChanged -= RecordMaxCombo;
            };
        }

        public static void OnSessionFinished() => _unsubscribeFromEvents();

        public static void OnNextFloorEnter() {
            _scores.Add(ScoreController.Instance.Score);
            _maxCombos.Add(_currentMaxCombo);
            _currentMaxCombo = 0;
        }

        public static void AddCurrency(int value) => _currencyGained += value;
    }
}