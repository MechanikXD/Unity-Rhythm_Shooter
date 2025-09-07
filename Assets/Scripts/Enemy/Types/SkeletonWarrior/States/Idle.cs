using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior.States {
    public class Idle : EnemyState<SkeletonWarrior> {
        private float _currentIdleTime;

        public Idle(SkeletonWarrior enemy) : base(enemy) { }
        
        public override void EnterState() {
            _currentIdleTime = 0f;
            Enemy.PlayAnimation(Enemy.IdleAnimationKey);
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime < Enemy.IdleTime) return;

            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity)) ChangeState<Attack>();
            else ChangeState<ChasePlayer>();
        }
    }
}