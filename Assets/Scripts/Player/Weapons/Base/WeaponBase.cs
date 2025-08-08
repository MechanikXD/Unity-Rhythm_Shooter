using UnityEngine;

namespace Player.Weapons.Base {
    public abstract class WeaponBase : MonoBehaviour {
        [SerializeField] protected Animator _animator;
        [SerializeField] private bool _canDoDoubleAction;
        [SerializeField] protected float _maxShootDistance;

        public bool IsReloading { get; protected set; }
        public bool CanFastReload { get; protected set; }
        
        public bool CanDoDoubleAction => _canDoDoubleAction;
        
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

        public virtual void OnWeaponSelected() { }
        public virtual void OnWeaponDeselected() { }
        public virtual void WeaponUpdate() {}
    }
}