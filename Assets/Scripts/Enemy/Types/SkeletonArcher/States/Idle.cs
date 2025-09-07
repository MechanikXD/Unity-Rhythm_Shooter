using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher.States {
    public class Idle : EnemyState<SkeletonArcher> {
        private float _currentIdleTime;

        public Idle(SkeletonArcher enemy) : base(enemy) { }
        
        public override void EnterState() {
            _currentIdleTime = 0f;
            Enemy.PlayAnimation(Enemy.IdleAnimationKey);
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime < Enemy.IdleTime) return;
            
            if (Enemy.HasLineOfSightWithPlayer()) ChangeState<Attack>();
            else ChangeState<Reposition>();
        }
    }
}