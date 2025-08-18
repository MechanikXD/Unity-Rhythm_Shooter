using Interactable;
using UnityEngine;

namespace Core.Offerings.Components {
    public abstract class OfferingEffect : MonoBehaviour {
        [SerializeField] protected EffectType _type;
        // TODO: Create actual status class
        [SerializeField] protected int _status;
        [SerializeField] protected float _value;

        public float Value => _value;
        public int Status => _status;
        
        public abstract void SubscribeEffect();
        protected abstract void Effect(DamageableBehaviour damageable);
        public abstract void UnsubscribeEffect();
    }
}