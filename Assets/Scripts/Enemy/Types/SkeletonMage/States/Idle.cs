using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class Idle : EnemyState<SkeletonMage> {
        private float _currentIdleTime;

        public Idle(SkeletonMage enemy) : base(enemy) { }
        
        public override void EnterState() {
            _currentIdleTime = 0f;
            Enemy.PlayAnimation(Enemy.IdleAnimationKey);
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime < Enemy.IdleTime) return;
            
            if (Enemy.HasLineOfSightWithPlayer()) ChangeState<Cast>();
            else ChangeState<Teleport>();
        }
    }
}