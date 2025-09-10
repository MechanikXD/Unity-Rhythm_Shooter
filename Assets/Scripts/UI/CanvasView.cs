using UnityEngine;

namespace UI {
    [RequireComponent(typeof(Canvas))]
    public abstract class CanvasView : MonoBehaviour {
        [SerializeField] private bool _disableOnStart = true;
        [SerializeField] protected Canvas _thisCanvas;

        public bool DisableOnStart => _disableOnStart;

        public virtual void EnterCanvas() {
            _thisCanvas.enabled = true;
        }

        public virtual void ExitCanvas() {
            _thisCanvas.enabled = false;
        }
    }
}