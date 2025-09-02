using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class TankIdle : EnemyState {
        private readonly float _idleTime;
        private readonly string _animationKey;
        private float _currentIdleTime;
        
        public TankIdle(EnemyBase enemy, float idleTime, string animationKey) : base(enemy) {
            _idleTime = idleTime;
            _animationKey = animationKey;
        }

        public override void EnterState() {
            Enemy.PlayAnimation(_animationKey);
            _currentIdleTime = 0f;
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;
            
            if (_currentIdleTime < _idleTime) return;

            if (Enemy.IsNearPlayer(1.5f)) ChangeState<AttackState>();
            else ChangeState<WalkTowardPlayer>();
        }
    }
}