using System;
using System.Collections.Generic;
using Core.Offerings.Definitions.General.Builder.Components;
using Enemy;
using Player;

namespace Core.Offerings.Definitions.General.Builder {
    public static class Builder {
        private readonly static IDictionary<EventTrigger, Func<Action<object>, Action>>
            Subscriber =
                new Dictionary<EventTrigger, Func<Action<object>, Action>> {
                    [EventTrigger.DamageDealt] = action => {
                        PlayerEvents.DamageDealt += action;
                        return () => PlayerEvents.DamageDealt -= action;
                    },
                    [EventTrigger.EnemyDefeated] = action => {
                        EnemyEvents.EnemyDefeated += action;
                        return () => EnemyEvents.EnemyDefeated -= action;
                    }
                };

        public static Action SubscribeToEvent(EventTrigger trigger, Action<object> action) {
            return !Subscriber.ContainsKey(trigger) ? () => {} : Subscriber[trigger](action);
        }
    }
}