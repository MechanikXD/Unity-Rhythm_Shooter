using Core.Offerings.Components;
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
        [SerializeField] private bool _inStartingHand;
        [SerializeField] private OfferingBase[] _conflictOfferings;
        [SerializeField] private OfferingBase[] _unlockOfferings;

        [Header("Logic")]
        [SerializeField] private OfferingEffect _effect;

        public Image Art => _art;
        public string Title => _title;
        public string Description => _description;
        public OfferingAffinity Affinity => _affinity;

        public bool InStartingHand => _inStartingHand;
        public OfferingBase[] ConflictOfferings => _conflictOfferings;
        public OfferingBase[] UnlockOfferings => _unlockOfferings;

        public void Apply() => _effect.SubscribeEffect();

        public void Remove() => _effect.UnsubscribeEffect();
    }
}