using Core.Behaviour.FiniteStateMachine;
using DG.Tweening;
using Enemy.Base;
using Interactable;
using Player.Statistics.Score;
using UnityEngine;

namespace Enemy.Types.Test {
    public class TestEnemy : EnemyBase {
        private Material _enemyMaterial;
        [SerializeField] private float _hitIndicatorFadeOff;
        private Tweener _materialColorAnimation;

        protected override void Awake() {
            base.Awake();
            
            IsTarget = false;
            EnemyStateMachine = new StateMachine();
            _enemyMaterial = GetComponent<MeshRenderer>().material;
            _enemyMaterial.color = Color.white;
        }

        protected override void EnterParriedState() => Debug.Log("Test Enemy Staggered");

        public override void TakeDamage(DamageInfo damageInfo) {
            base.TakeDamage(damageInfo);
            ScoreController.Instance.AddScore(10);
            
            if (_materialColorAnimation is { active: true }) _materialColorAnimation.Kill();

            var originalEnemyColor = _enemyMaterial.color;
            _enemyMaterial.color = Color.red;
            _materialColorAnimation = _enemyMaterial.DOColor(originalEnemyColor, _hitIndicatorFadeOff);
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