using Interactable.Damageable;
using Interactable.Status;
using UnityEngine;

namespace Core.Offerings.Components {
    public abstract class OfferingEffect : ScriptableObject {
        [SerializeField] protected StatusEffect _status;
        [SerializeField] protected float _value;

        public float Value => _value;
        public StatusEffect Status => _status;
        
        public abstract void SubscribeEffect();
        protected abstract void Effect(DamageableBehaviour damageable);
        public abstract void UnsubscribeEffect();

        protected void ApplyStatusOnTargets(DamageInfo info) {
            foreach (var target in info.Targets) {
                target.ApplyStatus(_status);  
            }
        }
    }
}