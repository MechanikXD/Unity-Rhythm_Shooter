using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonTank.States;
using Interactable.AttackCollider;
using UnityEngine;
using AttackState = Enemy.Types.SkeletonTank.States.AttackState;

namespace Enemy.Types.SkeletonTank {
    public class SkeletonTank : EnemyBase {
        [SerializeField] private EnemyAttackCollider _attackCollider;
        
        private const string IdleAnimationKey = "Paladin Idle";
        private const string WalkAnimationKey = "Paladin Walk";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _blockStart;
        private const string BlockLoopKey = "Block Loop";
        private const string BlockExitKey = "Block Exit";

        [SerializeField] private AnimationClip _attackWindUp;
        [SerializeField] private AnimationClip _attack;
        
        private readonly static int SlamWindUp = Animator.StringToHash("SlamWindUp");
        private readonly static int SlamAttack = Animator.StringToHash("SlamAttack");

        protected override EnemyState[] InitializeStates() {
            var idleState = new TankIdle(this, 1.5f, IdleAnimationKey);
            var shieldIdle = new ShieldIdle( this,3f, _blockStart, BlockLoopKey, BlockExitKey);
            
            var attackState = new AttackState(this, _attackWindUp, _attack, _attackCollider);

            var walkToPlayer = new WalkTowardPlayer(this, 3f, WalkAnimationKey);

            return new EnemyState[] {
                idleState,
                shieldIdle,
                attackState,
                walkToPlayer
            };
        }

        protected override void UpdateAnimationSpeed() {
            var crotchet = Conductor.Instance.SongData.Crotchet;
            
            Animator.SetFloat(SlamWindUp, _attackWindUp.length / crotchet);
            Animator.SetFloat(SlamAttack, _attack.length / (2 * crotchet));
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