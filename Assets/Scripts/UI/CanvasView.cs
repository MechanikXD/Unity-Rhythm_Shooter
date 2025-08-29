using UnityEngine;

namespace UI {
    [RequireComponent(typeof(Canvas))]
    public abstract class CanvasView : MonoBehaviour {
        protected Canvas ThisCanvas;
        
        protected virtual void Awake() {
            ThisCanvas = GetComponent<Canvas>();
        }

        public virtual void EnterCanvas() {
            ThisCanvas.enabled = true;
        }

        public virtual void ExitCanvas() {
            ThisCanvas.enabled = false;
        }
    }
}