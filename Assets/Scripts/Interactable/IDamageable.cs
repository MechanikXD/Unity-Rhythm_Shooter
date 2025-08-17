namespace Interactable {
    public interface IDamageable {
        public int CurrentHealth { get; protected set; }
        // TODO: Create actual status class
        public int CurrentStatuses { get; }
        
        public void TakeDamage(DamageInfo damageInfo);

        public void Parried(DamageInfo damageInfo);

        public void TakeDirectDamage(int value) => CurrentHealth -= value;
        // TODO: Create actual status class
        public void ApplyStatus(int status);

        public void Die();
    }
}