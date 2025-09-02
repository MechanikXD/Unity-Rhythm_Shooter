using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior.States {
    public class WarriorIdle : EnemyState {
        private readonly string _idleAnimationKey;
        private readonly float _idleTime;
        private float _currentIdleTime;

        public WarriorIdle(EnemyBase enemy, float idleTime, string idleAnimationKey) : base(enemy) {
            _idleTime = idleTime;
            _idleAnimationKey = idleAnimationKey;
        }
        
        public override void EnterState() {
            _currentIdleTime = 0f;
            Enemy.PlayAnimation(_idleAnimationKey);
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime < _idleTime) return;

            if (Enemy.IsNearPlayer(1.5f)) ChangeState<AttackState>();
            else ChangeState<ChasePlayer>();
        }
    }
}