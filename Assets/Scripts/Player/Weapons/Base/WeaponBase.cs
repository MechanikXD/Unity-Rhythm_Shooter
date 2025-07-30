using Core.Music;

namespace Player.Weapons.Base {
    public abstract class WeaponBase {
        protected bool InReload;
        protected bool InFireDelay;
        
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

        public abstract void OnReload();
        // On beat hit, give opportunity to fast reload
        protected abstract void StartReload();
        // On Miss, start slow reload
        protected abstract void StartSlowReload();
        protected abstract void FastReload();
        protected abstract void ContinueWithSlowReload();

        public virtual void OnWeaponSelected() { }
        public virtual void OnWeaponDeselected() { }
        public virtual void WeaponUpdate() {}
    }
}