using System.Linq;
using DG.Tweening;
using Enemy.Base;
using Enemy.States.Base;
using Enemy.States.General;
using Interactable.Damageable;
using Player.Statistics.Score;
using UnityEngine;

namespace Enemy.Types.Test {
    public class TestEnemy : EnemyBase {
        private Material _enemyMaterial;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private float _hitIndicatorFadeOff;
        private Tweener _materialColorAnimation;
        private EnemyState[] _myStates;

        protected override void Awake() {
            base.Awake();
            var idle = new IdleState(EnemyStateMachine, this, null, 15);
            var chase = new ChasePlayer(EnemyStateMachine, this, new EnemyState[] { idle }, _moveSpeed);
            idle.SetOutStates(new EnemyState[] { chase });
            
            _myStates = new EnemyState[] { idle, chase };
            
            EnemyStateMachine.Initialize(idle);
            
            IsTarget = false;
            _enemyMaterial = _renderer.material;
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

            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyEvents.OnEnemyDefeated(info);
            if (IsTarget) EnemyEvents.OnTargetDefeated(info);
            else EnemyEvents.OnNormalDefeated(info);
            
            Destroy(gameObject);
        }

        public override bool HasState<T>() => 
            _myStates.Any(state => state.GetType() == typeof(T));
    }
}