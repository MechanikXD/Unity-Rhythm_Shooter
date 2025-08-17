using System.Collections.Generic;
using Core.Offerings.Effect;
using Core.Offerings.Target;
using Core.Offerings.Trigger;
using Interactable;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Offerings {
    [CreateAssetMenu(fileName = "OfferingBase", menuName = "Scriptable Objects/Offering")]
    public class OfferingBase : ScriptableObject {
        [Header("Visual")]
        [SerializeField] private Image _art;
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private OfferingAffinity _affinity;
        
        [Header("Deck Related")]
        [SerializeField] private OfferingBase[] _conflictOfferings;
        [SerializeField] private OfferingBase[] _unlockOfferings;

        [Header("Logic")]
        [SerializeField] private OfferingTrigger[] _triggers;
        [SerializeField] private OfferingTarget[] _targets;
        // [SerializeField] private OfferingCondition[] _conditions;
        [SerializeField] private OfferingEffect[] _effects;

        public Image Art => _art;
        public string Title => _title;
        public string Description => _description;
        public OfferingAffinity Affinity => _affinity;
        
        public OfferingBase[] ConflictOfferings => _conflictOfferings;
        public OfferingBase[] UnlockOfferings => _unlockOfferings;

        public void Apply() {
            var targets = new List<IDamageable>();
            foreach (var target in _targets) {
                targets.AddRange(target.GetTargets());
            }
            
            // if conditions are met:
            
            
        }
    }
}