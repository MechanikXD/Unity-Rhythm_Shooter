using System;
using System.Collections.Generic;
using Enemy;
using Enemy.Base;
using Interactable.Damageable;
using Player;

namespace Core.Offerings.Definitions.General.Builder {
    public static class OfferingBuilder {
        private readonly static Dictionary<EventTrigger, object> EventSubscribers =
            new Dictionary<EventTrigger, object>();

        static OfferingBuilder() {
            EventSubscribers[EventTrigger.DamageDealt] = new Func<Action<DamageInfo>, Action>(action => {
                PlayerEvents.DamageDealt += action;
                return () => PlayerEvents.DamageDealt -= action;
            });
    
            EventSubscribers[EventTrigger.EnemyDefeated] = new Func<Action<EnemyBase>, Action>(action => {
                EnemyEvents.EnemyDefeated += action;
                return () => EnemyEvents.EnemyDefeated -= action;
            });
        }
        
        /// <param name="trigger"> Event that will be subscribed to (defined in EventTrigger enum) </param>
        /// <param name="action"> Action to subscribe </param>
        /// <typeparam name="TParam"> Type of parameters that action takes </typeparam>
        /// <returns> Action that will unsubscribe from event </returns>
        public static Action SubscribeToEvent<TParam>(EventTrigger trigger, Action<TParam> action) {
            if (!EventSubscribers.ContainsKey(trigger)) return () => {};
    
            var subscriber = (Func<Action<TParam>, Action>)EventSubscribers[trigger];
            return subscriber(action);
        }
    }
}