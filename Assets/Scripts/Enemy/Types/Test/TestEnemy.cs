using Core.Behaviour.FiniteStateMachine;
using DG.Tweening;
using Enemy.Base;
using Player.Statistics.Score;
using UnityEngine;

namespace Enemy.Types.Test {
    public class TestEnemy : EnemyBase {
        private Material _enemyMaterial;
        [SerializeField] private float _hitIndicatorFadeOff;
        private Tweener _materialColorAnimation;
        
        protected void Awake() {
            IsTarget = false;
            EnemyStateMachine = new StateMachine();
            _enemyMaterial = GetComponent<MeshRenderer>().material;
            _enemyMaterial.color = Color.white;
            _maxHealth = 5;
            CurrentHealth = 5;
        }
        
        public override void TakeDamage(int value) {
            ScoreController.Instance.AddScore(10);
            
            if (_materialColorAnimation is { active: true }) _materialColorAnimation.Kill();

            var originalEnemyColor = _enemyMaterial.color;
            _enemyMaterial.color = Color.red;
            _materialColorAnimation = _enemyMaterial.DOColor(originalEnemyColor, _hitIndicatorFadeOff);

            CurrentHealth -= value;
            if (CurrentHealth <= 0) Die();
        }

        public override void Die() {
            ScoreController.Instance.AddScore(50);
            
            EnemyEvents.OnEnemyDefeated();
            if (IsTarget) EnemyEvents.OnTargetDefeated();
            else EnemyEvents.OnNormalDefeated();
            
            Destroy(gameObject);
        }
    }
}