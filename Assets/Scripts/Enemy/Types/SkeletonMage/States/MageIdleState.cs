using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonMage.States {
    public class MageIdleState : EnemyState {
        private readonly string _idleAnimationKey;
        private float _idleTime;
        private float _currentIdleTime;

        public MageIdleState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            float idleTime, string idleAnimationKey) : base(stateMachine, enemy, outStates) {
            _idleTime = idleTime;
            _idleAnimationKey = idleAnimationKey;
        }
        
        public void SetIdleTime(float time) => _idleTime = time;
        
        public override void EnterState() {
            _currentIdleTime = 0f;
            Enemy.PlayAnimation(_idleAnimationKey);
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime >= _idleTime) {
                AttachedStateMachine.ChangeState(Enemy.HasLineOfSightWithPlayer()
                    ? OutStates[0] // Teleport elsewhere
                    : OutStates[1]); // Attack state
            }
        }
    }
}