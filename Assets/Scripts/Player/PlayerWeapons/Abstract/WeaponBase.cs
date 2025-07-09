namespace Player.PlayerWeapons.Abstract {
    public abstract class WeaponBase {
        public abstract void LeftAction();
        public abstract void RightAction();
        
        public virtual void WeaponInit() {}
        public virtual void WeaponUpdate() {}
    }
}