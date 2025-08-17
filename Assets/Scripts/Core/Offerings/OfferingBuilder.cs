using System;
using System.Collections.Generic;
using Core.Offerings.Effect;
using Core.Offerings.Target;
using Core.Offerings.Trigger;
using Interactable;
using Player;

namespace Core.Offerings {
    public static class OfferingBuilder {
        private static Dictionary<TriggerType, Action<Action>> _subscriber;
        private static Dictionary<TargetType, DamageableBehaviour[]> _targetGetter;
        private static Dictionary<EffectorType, Action<SessionModel.ValueModifier, float>> _setters;
        // private HashSet<TargetType> _setter;

        public static void Initialize() {
            _setters = new Dictionary<EffectorType, Action<SessionModel.ValueModifier, float>> {
                [EffectorType.IncreaseFlat] = (modifier, value) => modifier.FlatModifier += value,
                [EffectorType.IncreasePercent] = (modifier, value) => modifier.PercentModifier += value,
                [EffectorType.DecreaseFlat] = (modifier, value) => modifier.FlatModifier -= value,
                [EffectorType.DecreasePercent] = (modifier, value) => modifier.PercentModifier += value,
            };
        }

        public static void SubscribeAction(TriggerType trigger, Action action) =>
            _subscriber[trigger](action);

        public static DamageableBehaviour[] GetTargets(TargetType target) => _targetGetter[target];
        
    }
}