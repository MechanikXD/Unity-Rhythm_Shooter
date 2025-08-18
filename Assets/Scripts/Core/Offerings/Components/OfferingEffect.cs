using Interactable.Damageable;
using Interactable.Status;
using UnityEngine;

namespace Core.Offerings.Components {
    public abstract class OfferingEffect : MonoBehaviour {
        [SerializeField] protected StatusBase _status;
        [SerializeField] protected float _value;

        public float Value => _value;
        public StatusBase Status => _status;
        
        public abstract void SubscribeEffect();
        protected abstract void Effect(DamageableBehaviour damageable);
        public abstract void UnsubscribeEffect();
    }
}