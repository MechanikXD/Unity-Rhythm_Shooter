using System;
using Core.Offerings.Builder;
using Core.Offerings.Components;
using Interactable.Damageable;
using UnityEngine;

namespace GameEffects.Offering_Effects {
    [CreateAssetMenu(fileName = "InflictStatusOnEnemy", menuName = "Scriptable Objects/Offering Effect/Action Inflict Status")]
    public class InflictStatusOnEnemy : OfferingEffect {
        [SerializeField] private EventTrigger _trigger;

        private Action _unSubscriber;
        
        public override void SubscribeEffect() => 
            _unSubscriber = 
                OfferingBuilder.SubscribeToEvent<DamageInfo>(_trigger, ApplyStatusOnTargets);

        protected override void Effect(DamageableBehaviour damageable) { }

        public override void UnsubscribeEffect() => _unSubscriber();
    }
}