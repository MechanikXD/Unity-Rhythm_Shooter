using System;
using System.Collections.Generic;
using Core.Game;
using Core.Offerings.Components;
using Interactable;
using UnityEngine;

namespace Core.Offerings {
    public static class OfferingBuilder {
        private static Dictionary<TargetType, Func<DamageableBehaviour[]>> _targetGetter;
        private static Dictionary<EffectType, Action<DamageableBehaviour, OfferingEffect>> _setters;

        public static void Initialize() {
            _setters = new Dictionary<EffectType, Action<DamageableBehaviour, OfferingEffect>> {
                [EffectType.FlatHealth] = (damageable, value) => damageable.ChangeHealthIncrement(
                    (int)(damageable.HealthIncrement + value.Value)),
                [EffectType.FlatDamage] = (damageable, value) => damageable.ChangeDamageIncrement(
                    (int)(damageable.DamageIncrement + value.Value)),
                [EffectType.HealthPercent] = (damageable, value) => damageable.ChangeHealthMultiplier(
                    (int)(damageable.HealthMultiplier + value.Value)),
                [EffectType.DamagePercent] = (damageable, value) => damageable.ChangeDamageMultiplier(
                    (int)(damageable.DamageMultiplier + value.Value)),
                [EffectType.DamageReduction] = (damageable, value) => damageable.SetDamageReduction(
                    damageable.CurrentDamageReduction + value.Value),
                [EffectType.ApplyStatus] = (damageable, value) => damageable.ApplyStatus(value.Status)
            };

            var player = GameManager.Instance.Player;
            _targetGetter = new Dictionary<TargetType, Func<DamageableBehaviour[]>> {
                [TargetType.Player] = () => new DamageableBehaviour[] { player },
                [TargetType.AllEnemies] = () => GameManager.Instance.ActiveEnemies,
                [TargetType.ClosestEnemy] = () => {
                    DamageableBehaviour min = null;
                    var minDist = float.MaxValue;
                    var enemies = GameManager.Instance.ActiveEnemies;
                    foreach (var enemy in enemies) {
                        var currentDist = Vector3.Distance(enemy.Position, player.Position);
                        if (currentDist >= minDist) continue;

                        minDist = currentDist;
                        min = enemy;
                    }

                    return new[] { min };
                },
            };
        }

        public static Func<DamageableBehaviour[]> GetTargets(TargetType target) => 
            _targetGetter[target];

        public static Action<DamageableBehaviour, OfferingEffect> GetValueSetter(EffectType type) => 
            _setters[type];
    }
}