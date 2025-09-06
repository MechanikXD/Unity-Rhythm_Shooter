using System;
using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using UnityEngine;

namespace Enemy.Base {
    public abstract class EnemyState<TEnemy> : State where TEnemy : EnemyBase {
        protected readonly TEnemy Enemy;

        protected EnemyState(TEnemy enemy) : base(enemy.StateMachine) => Enemy = enemy;

        protected IEnumerator ForceExitStateAfter(float delay, Type state) {
            yield return new WaitForSeconds(delay);
            AttachedStateMachine.ChangeState(Enemy.States[state]);
        }
        
        protected IEnumerator ForceExitStateAfter<T>(float delay) where T : EnemyState<TEnemy> {
            yield return new WaitForSeconds(delay);
            ChangeState<T>();
        }

        protected void ChangeState<T>() where T : EnemyState<TEnemy> {
            AttachedStateMachine.ChangeState(Enemy.States[typeof(T)]);
        }
    }
}