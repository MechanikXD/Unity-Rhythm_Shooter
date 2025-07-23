using Core.StateMachine.Base;
using DG.Tweening;
using Enemy.Base;
using Player.Statistics.Score;
using UnityEngine;

namespace Enemy.Types.Test {
    public class TestEnemy : EnemyBase {
        [SerializeField] private Material _enemyMaterial;
        [SerializeField] private float _hitIndicatorFadeOff;
        private Tweener _materialColorAnimation;
        
        protected void Awake() {
            EnemyStateMachine = new StateMachine();
            _enemyMaterial.color = Color.white;
            _maxHealth = -1;
            CurrentHealth = -1;
        }
        
        public override void TakeDamage(int value) {
            ScoreController.Instance.AddScore(10);
            
            if (_materialColorAnimation is { active: true }) _materialColorAnimation.Kill();

            var originalEnemyColor = _enemyMaterial.color;
            _enemyMaterial.color = Color.red;
            _materialColorAnimation = _enemyMaterial.DOColor(originalEnemyColor, _hitIndicatorFadeOff);
        }

        public override void Die() => throw new System.NotImplementedException();
    }
}