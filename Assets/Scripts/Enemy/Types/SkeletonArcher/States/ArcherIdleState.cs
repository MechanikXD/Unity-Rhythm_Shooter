using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher.States {
    public class ArcherIdleState : EnemyState {
        private readonly string _idleAnimationKey;
        private readonly float _idleTime;
        private float _currentIdleTime;

        public ArcherIdleState(EnemyBase enemy, float idleTime, string idleAnimationKey) : base(enemy) {
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
            
            if (Enemy.HasLineOfSightWithPlayer()) ChangeState<AttackState>();
            else ChangeState<RepositionState>();
        }
    }
}