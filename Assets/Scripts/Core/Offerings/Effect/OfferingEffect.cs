using System;
using Interactable;
using UnityEngine;

namespace Core.Offerings.Effect {
    [Serializable]
    public class OfferingEffect {
        [SerializeField] private string _uniqueEffectName;
        [SerializeField] private EffectType _effect;
        [SerializeField] private EffectorType _effector;
        // TODO: Create actual status class
        [SerializeField] private int _status;
        [SerializeField] private float _value;

        public int Status => _status;
        public string Name => _uniqueEffectName;

        public void ApplyEffect(IDamageable[] targets, OfferingEffect[] effects) {
            foreach (var target in targets) {
                
            }
        }
    }
}