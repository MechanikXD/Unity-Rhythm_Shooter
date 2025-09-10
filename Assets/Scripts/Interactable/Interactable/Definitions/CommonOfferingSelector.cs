using UI;
using UI.Views.OfferingSelection;

namespace Interactable.Interactable.Definitions {
    public class CommonOfferingSelector : InteractableBase {
        public override void Interact() {
            Destroy(gameObject);
            UIManager.Instance.EnterUICanvas<OfferingView>();
        }
    }
}