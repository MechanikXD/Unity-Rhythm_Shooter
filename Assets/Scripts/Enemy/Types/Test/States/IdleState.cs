using Core.Behaviour.FiniteStateMachine;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.Test.States {
    public class IdleState : EnemyState {
        private float _idleTime;
        private float _currentIdleTime;

        public IdleState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            float idleTime) : base(stateMachine, enemy, outStates) {
            _idleTime = idleTime;
        }

        public void SetIdleTime(float time) => _idleTime = time;
        
        public override void EnterState() {
            _currentIdleTime = 0f;
        }

        public override void FrameUpdate() {
            _currentIdleTime += Time.deltaTime;

            if (_currentIdleTime >= _idleTime) 
                AttachedStateMachine.ChangeState(OutStates[0]);
        }

    }
}