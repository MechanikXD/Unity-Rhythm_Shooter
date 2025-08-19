using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Offerings {
    public static class OfferingManager {
        private static HashSet<OfferingBase> _allOfferings;
        private static HashSet<OfferingBase> _possibleOfferings;
        private static HashSet<OfferingBase> _blockedOfferings;
        private static HashSet<OfferingBase> _activeOfferings;

        public static void Initialize() {
            _allOfferings = GameManager.Instance.AllOfferings.ToHashSet();
            _possibleOfferings = new HashSet<OfferingBase>();
            _blockedOfferings = new HashSet<OfferingBase>();
            _activeOfferings = new HashSet<OfferingBase>();
        }

        public static void CreateNewHand() {
            _activeOfferings.Clear();
            _blockedOfferings.Clear();
            _possibleOfferings.Clear();
            _possibleOfferings = _allOfferings.Where(o => o.InStartingHand).ToHashSet();
        }

        public static OfferingBase[] Offer(int count) {
            var shuffled = _possibleOfferings.ToList();
            shuffled.Shuffle();

            if (shuffled.Count <= 0) {
                // TODO: Create offering that appears when all possible offerings taken
                return Array.Empty<OfferingBase>();
            }

            var actualCount = Mathf.Min(Mathf.Min(count, 4), shuffled.Count);
                
            var offers = new OfferingBase[actualCount];
            for (var i = 0; i < actualCount; i++) offers[i] = shuffled[i];

            return offers;
        }

        public static void SelectOffering(OfferingBase offering) {
            if (!_activeOfferings.Contains(offering) || !_possibleOfferings.Contains(offering)) return;

            _blockedOfferings.UnionWith(offering.ConflictOfferings);
            _possibleOfferings.UnionWith(
                offering.UnlockOfferings.Where(o =>
                    !_blockedOfferings.Contains(o) && !_activeOfferings.Contains(o)));

            offering.Apply();
            _activeOfferings.Add(offering);
        }

        public static void RemoveOffering(OfferingBase offering) {
            if (!_activeOfferings.Contains(offering)) return;

            _activeOfferings.Remove(offering);
            
            _blockedOfferings.Clear();
            foreach (var active in _activeOfferings) {
                _blockedOfferings.UnionWith(active.ConflictOfferings);
            }
            
            _possibleOfferings.ExceptWith(offering.UnlockOfferings);
            foreach (var active in _activeOfferings) {
                _possibleOfferings.UnionWith(active.UnlockOfferings.Where(o =>
                    !_blockedOfferings.Contains(o) && !_activeOfferings.Contains(o)));
            }
        }
        
        private static void Shuffle<T>(this IList<T> list) {
            for (var i = 0; i < list.Count; ++i) {
                var randomIndex = Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}