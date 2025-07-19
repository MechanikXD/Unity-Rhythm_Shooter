using Core.StateMachine.Base;
using DG.Tweening;
using Enemy.Base;
using Player.Statistics.Score;
using UnityEngine;

namespace Enemy.Types.Test {
    public class TestEnemy : EnemyBase {
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private float hitIndicatorFadeOff;
        private Tweener _materialColorAnimation;
        
        protected void Awake() {
            EnemyStateMachine = new StateMachine();
            enemyMaterial.color = Color.white;
            maxHealth = -1;
            CurrentHealth = -1;
        }
        
        public override void TakeDamage(int value) {
            ScoreController.Instance.AddScore(10);
            
            if (_materialColorAnimation is { active: true }) _materialColorAnimation.Kill();

            var originalEnemyColor = enemyMaterial.color;
            enemyMaterial.color = Color.red;
            _materialColorAnimation = enemyMaterial.DOColor(originalEnemyColor, hitIndicatorFadeOff);
        }

        public override void Die() => throw new System.NotImplementedException();
    }
}