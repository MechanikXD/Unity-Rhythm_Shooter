using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class Idle : EnemyState {
        private readonly float _idleTime;
        private readonly string _animationKey;
        private float _currentIdleTime;
        
        public Idle(EnemyBase enemy, float idleTime, string animationKey) : base(enemy) {
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

            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity)) ChangeState<Attack>();
            else ChangeState<ChasePlayer>();
        }
    }
}