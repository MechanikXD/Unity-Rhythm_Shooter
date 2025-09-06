using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class Idle : EnemyState<SkeletonTank> {
        private float _currentIdleTime;
        
        public Idle(SkeletonTank enemy) : base(enemy) { }

        public override void EnterState() {
            Enemy.PlayAnimation(Enemy.IdleAnimationKey);
            _currentIdleTime = 0f;
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;
            
            if (_currentIdleTime < Enemy.NormalIdleTime) return;

            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity)) ChangeState<Attack>();
            else ChangeState<ChasePlayer>();
        }
    }
}