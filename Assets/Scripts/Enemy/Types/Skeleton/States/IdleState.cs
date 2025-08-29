using Core.Behaviour.FiniteStateMachine;
using Core.Game;
using Enemy.Base;
using Enemy.States.Base;
using UnityEngine;

namespace Enemy.Types.Skeleton.States {
    public class IdleState : EnemyState {
        private readonly string _idleAnimationKey;
        private float _idleTime;
        private float _currentIdleTime;

        public IdleState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
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

            if (_currentIdleTime >= _idleTime) 
                AttachedStateMachine.ChangeState(OutStates[PlayerIsNearby() ? 0 : 1]);
        }

        private bool PlayerIsNearby() {
            return Vector3.Distance(Enemy.Position, GameManager.Instance.Player.Position) < 1.5f;
        }
    }
}