using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class Idle : EnemyState {
        private readonly string _idleAnimationKey;
        private readonly float _idleTime;
        private float _currentIdleTime;

        public Idle(EnemyBase enemy, float idleTime, string idleAnimationKey) : base(enemy) {
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
            
            if (Enemy.HasLineOfSightWithPlayer()) ChangeState<Cast>();
            else ChangeState<Teleport>();
        }
    }
}