using System;
using Core.Music;
using UnityEngine;

namespace Player.Weapons.Base {
    public abstract class WeaponBase : MonoBehaviour { 
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _crossFade;
        [SerializeField] private bool _canDoDoubleAction;
        [SerializeField] protected float _maxShootDistance;
        [SerializeField] protected int _maxAmmo;
        
        protected Func<Vector3, Ray> ScreenPointToRay;
        
        public bool IsReloading { get; protected set; }
        public bool CanFastReload { get; protected set; }
        
        public bool CanDoDoubleAction => _canDoDoubleAction;
        
        protected bool IsWalking;

        private Coroutine _reloadRemoveInAnimation;
        
        [Header("Animations")]
        [SerializeField] protected AnimationClip _walk;
        [SerializeField] protected AnimationClip _action;
        [SerializeField] protected AnimationClip _reloadStart;
        [SerializeField] protected AnimationClip _reloadSlow;
        [SerializeField] protected AnimationClip _reloadFast;

        protected readonly static int WalkSpeed = Animator.StringToHash("WalkSpeed");
        protected readonly static int ShootSpeed = Animator.StringToHash("ShootSpeed");
        private readonly static int ReloadStartSpeed = Animator.StringToHash("ReloadStartSpeed");
        private readonly static int ReloadSlowSpeed = Animator.StringToHash("ReloadSlowSpeed");
        private readonly static int ReloadFastSpeed = Animator.StringToHash("ReloadFastSpeed");

        protected static float HalfCrotchet;
        protected static float Crotchet;
        
        public abstract void LeftPerfectAction();
        public abstract void LeftGoodAction();
        public abstract void LeftMissedAction();
        
        public abstract void RightPerfectAction();
        public abstract void RightGoodAction();
        public abstract void RightMissedAction();

        public virtual void BothPerfectAction() {
            RightPerfectAction();
            LeftPerfectAction();
        }
        public virtual void BothGoodAction() {
            RightGoodAction();
            LeftGoodAction();
        }
        public virtual void BothMissedAction() {
            RightMissedAction();
            LeftMissedAction();
        }

        public abstract bool CanDoLeftAction();
        public abstract bool CanDoRightAction();
        public abstract bool CanDoBothAction();

        public abstract void StartReload();
        public abstract void FastReload();
        public abstract void SlowReload();

        public virtual void OnWeaponSelected() {
            if (Camera.main == null) Debug.LogError("No Main Camera was Found!");
            ScreenPointToRay = Camera.main!.ScreenPointToRay;

            HalfCrotchet = Conductor.Instance.SongData.HalfCrotchet;
            Crotchet = Conductor.Instance.SongData.Crotchet;
            
            CalculateAnimationsSpeed();
        }
        public virtual void OnWeaponDeselected() { }
        public virtual void WeaponUpdate() {}
        
        protected virtual void CalculateAnimationsSpeed() {
            // Values driven from original animation speed
            _animator.SetFloat(WalkSpeed, _walk.length / Crotchet);
            _animator.SetFloat(ShootSpeed, _action.length / HalfCrotchet);
            _animator.SetFloat(ReloadStartSpeed,  _reloadStart.length / (1.5f * Crotchet));
            _animator.SetFloat(ReloadSlowSpeed, _reloadSlow.length / (Crotchet * 2f));
            _animator.SetFloat(ReloadFastSpeed, _reloadFast.length / HalfCrotchet);
        }
    }
}