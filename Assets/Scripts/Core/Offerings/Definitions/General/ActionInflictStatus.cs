using System;
using Core.Offerings.Components;
using Core.Offerings.Definitions.General.Builder;
using Interactable.Damageable;
using UnityEngine;

namespace Core.Offerings.Definitions.General {
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