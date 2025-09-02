using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonArcher.States;
using UnityEngine;
using AttackState = Enemy.Types.SkeletonArcher.States.AttackState;

namespace Enemy.Types.SkeletonArcher {
    public class SkeletonArcher : EnemyBase {
        [SerializeField] private Transform _arrowSpawnPoint;
        [SerializeField] private EnemyArrow _enemyAttack;
        
        private const string IdleAnimationKey = "Archer Idle";
        private const string RunAnimationKey = "Archer Run";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        private readonly static int AttackStartSpeed = Animator.StringToHash("AttackStartSpeed");

        protected override EnemyState[] InitializeStates() {
            var idleState = new ArcherIdleState(this, 1, IdleAnimationKey);
            var repositionState = new RepositionState(this, RunAnimationKey, 5f);
            var attackState =
                new AttackState(this, _arrowSpawnPoint, _enemyAttack, _attackStateEnter.name, _attackStateLoop.name, _attackStateExit);

            return new EnemyState[] {
                idleState,
                repositionState,
                attackState
            };
        }

        protected override void UpdateAnimationSpeed() {
            _animator.SetFloat(AttackStartSpeed, 
                _attackStateEnter.length / Conductor.Instance.SongData.Crotchet);
        }

        public override void Die() {
            _animator.CrossFade(DeathAnimationKey, _crossFade, -1, 0f);
            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyEvents.OnEnemyDefeated(info);
            if (IsTarget) EnemyEvents.OnTargetDefeated(info);
            else EnemyEvents.OnNormalDefeated(info);
            
            Destroy(gameObject, DeathAnimationKey.Length);
        }
    }
}