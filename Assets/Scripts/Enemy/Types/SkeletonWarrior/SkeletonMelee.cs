using Core.Music;
using Enemy.Base;
using Enemy.States.Base;
using Enemy.Types.Skeleton.States;
using UnityEngine;

namespace Enemy.Types.Skeleton {
    public class SkeletonMelee : EnemyBase {
        private IdleState _idleState;
        private ComboState _comboState;
        private ChasePlayer _chaseState;
        
        [SerializeField] private string _idleAnimation;
        [SerializeField] private string _walkAnimation;
        [SerializeField] private string _deathAnimation;

        [SerializeField] private AnimationClip _windUp;
        [SerializeField] private AnimationClip _combo1;
        [SerializeField] private AnimationClip _combo2;
        [SerializeField] private AnimationClip _combo3;
        [SerializeField] private AnimationClip _fromCombo;
        
        private readonly static int Combo1Speed = Animator.StringToHash("Combo1Speed");
        private readonly static int Combo2Speed = Animator.StringToHash("Combo2Speed");
        private readonly static int Combo3Speed = Animator.StringToHash("Combo3Speed");

        protected override void Awake() {
            base.Awake();
            
            _idleState = new IdleState(EnemyStateMachine, this, null, 5, _idleAnimation);
            EnemyStateMachine.Initialize(_idleState);
        }

        protected void Start() {
            _comboState = new ComboState(EnemyStateMachine, this,
                new EnemyState[] { _idleState },
                new[] { _windUp, _combo1, _combo2, _combo3, _fromCombo });
            _chaseState = new ChasePlayer(EnemyStateMachine, this, new EnemyState[] { _comboState }, _walkAnimation);
            _idleState.SetOutStates(new EnemyState[] { _comboState, _chaseState });
            
            var crotchet = Conductor.Instance.SongData.Crotchet;
            _animator.SetFloat(Combo1Speed, _combo1.length / crotchet);
            _animator.SetFloat(Combo2Speed, _combo2.length / crotchet);
            _animator.SetFloat(Combo3Speed, _combo3.length / crotchet);
        }

        protected override void EnterParriedState() { }

        protected override void UpdateMoveSpeedOnCharacter() {
            _chaseState.SetMoveSpeed(CurrentSpeed);
        }

        public override void Die() {
            _animator.CrossFade(_deathAnimation, _crossFade, -1, 0f);
            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyEvents.OnEnemyDefeated(info);
            if (IsTarget) EnemyEvents.OnTargetDefeated(info);
            else EnemyEvents.OnNormalDefeated(info);
            
            Destroy(gameObject, _deathAnimation.Length);
        }
    }
}