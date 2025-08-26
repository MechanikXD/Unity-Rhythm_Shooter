using System.Collections.Generic;
using Interactable.Interactable;
using UnityEngine;

namespace Player.Interactions {
    public class PlayerInteractionTrigger : MonoBehaviour {
        [SerializeField] private Collider _thisCollider; 
        private List<InteractableBase> _currentInteractable;

        private void Awake() {
            _currentInteractable = new List<InteractableBase>();
        }

        private void FixedUpdate() {
            if (_currentInteractable.Count < 2) return;

            GetClosestInteractable();
        }

        private int GetClosestInteractable() {
            if (_currentInteractable.Count == 1) {
                _currentInteractable[0].HighLight();
                return 0;
            }

            var closestIndex = 0;
            var closestDist = float.MaxValue;
            for (var i = 0; i < _currentInteractable.Count; i++) {
                var distance = Vector3.Distance(_currentInteractable[i].Position,
                    _thisCollider.transform.position);
                if (distance < closestDist) {
                    closestDist = distance;
                    closestIndex = i;
                }
                _currentInteractable[i].UnderEmphasize();
            }
                
            _currentInteractable[closestIndex].HighLight();
            return closestIndex;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent<InteractableBase>(out var interactable)) return;
            
            if (_currentInteractable.Count == 0) {
                interactable.HighLight();
            }
            _currentInteractable.Add(interactable);
        }

        private void OnTriggerExit(Collider other) {
            if (!other.TryGetComponent<InteractableBase>(out var interactable)) return;

            if (_currentInteractable.Contains(interactable)) {
                _currentInteractable.Remove(interactable);
            }
            interactable.UnderEmphasize();

            if (_currentInteractable.Count == 1) {
                _currentInteractable[0].HighLight();
            }
        }

        public void Interact() {
            if (_currentInteractable.Count == 0) return;
            _currentInteractable[GetClosestInteractable()].Interact();
        }
    }
}