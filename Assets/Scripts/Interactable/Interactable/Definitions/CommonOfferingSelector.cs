using UI;
using UnityEngine;

namespace Interactable.Interactable.Definitions {
    public class CommonOfferingSelector : InteractableBase {
        public override void Interact() {
            Destroy(gameObject);
            UIManager.Instance.EnterOfferingSelection();
        }
    }
}