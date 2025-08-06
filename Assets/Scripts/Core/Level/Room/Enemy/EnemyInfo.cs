using System;
using Enemy.Base;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Level.Room.Enemy {
    [Serializable]
    public class EnemyInfo {
        [SerializeField] private SpawnLogic _spawnLogic;

        [Header("Waves")]
        [SerializeField] private Wave<EnemyBase>[] _waves;
        private int _currentWave;
        private int _currentEnemyIndex;

        [Header("Wave of Randoms")]
        [Tooltip("Note: array above is used for enemy selection")]
        [SerializeField] private int[] _amountOfEnemiesPerWave;
        private int _currentEnemyCount;

        [Header("Random")]
        [SerializeField] private EnemyBase[] _enemyPool;
        [SerializeField] private int _enemyAmount;

        [Header("Bounty")]
        [SerializeField] private EnemyBase[] _targets;
        private int _targetsDefeated;

        [Header("Bounty With Odds")]
        [SerializeField] private string _note = "Use array of targets above and enemy pool";

        public bool IsWaveBased => _spawnLogic is SpawnLogic.Waves or SpawnLogic.RandomWaves;
        public bool IsBountyBased => _spawnLogic is SpawnLogic.Bounty or SpawnLogic.BountyWithOdds;
        public bool AllTargetsDefeated => !IsBountyBased || _targets.Length - _targetsDefeated == 0;

        public bool AdvanceWave() {
            _currentWave++;
            _currentEnemyIndex = 0;
            _currentEnemyCount = 0;
            return _currentWave < _waves.Length;
        }

        public void TargetDefeated() {
            _targetsDefeated++;
        }

        public EnemyBase GetNextEnemy() {
            return _spawnLogic switch {
                SpawnLogic.Waves => NextWaveEnemy(),
                SpawnLogic.Random => NextRandomEnemy(),
                SpawnLogic.RandomWaves => NextWaveRandomEnemy(),
                SpawnLogic.Bounty => NextBountyTarget(),
                SpawnLogic.BountyWithOdds => NextBountyTargetWithOdds(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private EnemyBase NextWaveEnemy() {
            if (_currentEnemyIndex >= _waves[_currentWave].Count) return null;
            
            var next = _waves[_currentWave][_currentEnemyIndex];
            _currentEnemyIndex++;
            return next;
        }

        private EnemyBase NextWaveRandomEnemy() {
            if (_currentEnemyCount >= _amountOfEnemiesPerWave[_currentWave]) return null;
            
            var next = _waves[_currentWave][Random.Range(0, _waves[_currentWave].Count)];
            _currentEnemyCount++;
            return next;
        }

        private EnemyBase NextRandomEnemy() {
            if (_currentEnemyCount >= _enemyAmount) return null;

            var next = _enemyPool[Random.Range(0, _enemyPool.Length)];
            _currentEnemyCount++;
            return next;
        }

        private EnemyBase NextBountyTarget() {
            if (_currentEnemyIndex >= _targets.Length) return null;

            var next = _targets[_currentEnemyIndex];
            next.SetIsTarget();
            _currentEnemyIndex++;
            return next;
        }

        private EnemyBase NextBountyTargetWithOdds() {
            var nextTarget = NextBountyTarget();
            if (nextTarget == null && !AllTargetsDefeated) {
                nextTarget = NextRandomEnemy();
            }

            return nextTarget;
        }
    }
}