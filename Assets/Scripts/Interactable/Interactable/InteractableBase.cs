using UnityEngine;

namespace Interactable.Interactable {
    public abstract class InteractableBase : MonoBehaviour {
        public Vector3 Position => transform.position;
        private bool _isHighLighted;
        
        public virtual void HighLight() {
            if (_isHighLighted) return;
            
            // Do stuff
        }
        
        public virtual void UnderEmphasize() {
            if (!_isHighLighted) return;
            
            // Do stuff
        }

        public abstract void Interact();
    }
}