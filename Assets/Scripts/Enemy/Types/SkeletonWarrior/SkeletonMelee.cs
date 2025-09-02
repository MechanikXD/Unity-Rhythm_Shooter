using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonWarrior.States;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior {
    public class SkeletonMelee : EnemyBase {
        private const string IdleAnimationKey = "Skele Idle";
        private const string WalkAnimationKey = "Skele Walk";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _windUp;
        [SerializeField] private AnimationClip _combo1;
        [SerializeField] private AnimationClip _combo2;
        [SerializeField] private AnimationClip _combo3;
        [SerializeField] private AnimationClip _fromCombo;
        
        private readonly static int Combo1Speed = Animator.StringToHash("Combo1Speed");
        private readonly static int Combo2Speed = Animator.StringToHash("Combo2Speed");
        private readonly static int Combo3Speed = Animator.StringToHash("Combo3Speed");

        protected override EnemyState[] InitializeStates() {
            var idleState = new WarriorIdle(this, 2, IdleAnimationKey);
            var retreat = new Retreat(this, _moveSpeed, WalkAnimationKey);
            var attackState = new AttackState(this, new[] { _windUp, _combo1, _combo2, _combo3, _fromCombo });
            var chaseState = new ChasePlayer(this, WalkAnimationKey);

            return new EnemyState[] {
                idleState,
                retreat,
                attackState,
                chaseState
            };
        }

        protected override void UpdateAnimationSpeed() {
            var crotchet = Conductor.Instance.SongData.Crotchet;
            _animator.SetFloat(Combo1Speed, _combo1.length / crotchet);
            _animator.SetFloat(Combo2Speed, _combo2.length / crotchet);
            _animator.SetFloat(Combo3Speed, _combo3.length / crotchet);
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