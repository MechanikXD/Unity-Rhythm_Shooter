using System;
using Interactable;
using UnityEngine;

namespace Core.Offerings.Target {
    [Serializable]
    public class OfferingTarget {
        [SerializeField] private TargetType _targetType;

        public IDamageable[] GetTargets() => OfferingBuilder.GetTargets(_targetType);
    }
}