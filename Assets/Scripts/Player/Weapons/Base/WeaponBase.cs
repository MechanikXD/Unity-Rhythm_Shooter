using Core.Music;

namespace Player.Weapons.Base {
    public abstract class WeaponBase {
        public abstract void LeftPerfectAction();
        public abstract void LeftGoodAction();
        public virtual void LeftMissedAction() {
            Conductor.Instance.DisableNextInteractions(2);
        }
        
        public abstract void RightPerfectAction();
        public abstract void RightGoodAction();
        public virtual void RightMissedAction() {
            Conductor.Instance.DisableNextInteractions(2);
        }

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

        public virtual void OnWeaponSelected() { }
        public virtual void OnWeaponDeselected() { }
        public virtual void WeaponUpdate() {}
    }
}