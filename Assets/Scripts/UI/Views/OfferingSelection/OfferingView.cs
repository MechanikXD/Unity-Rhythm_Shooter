using Core.Offerings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.OfferingSelection {
    public class OfferingView : MonoBehaviour, ICanvasView {
        [SerializeField] private OfferingSprite[] _spritePool;
        [SerializeField] private Button _forgetButton;
        private Canvas _thisCanvas;

        private void OnEnable() {
            // TODO: Not correct exit, UI manager should handle it. 
            _forgetButton.onClick.AddListener(ExitCanvas);
        }

        private void Awake() {
            _thisCanvas = GetComponent<Canvas>();
        }

        private void OnDisable() {
            _forgetButton.onClick.RemoveListener(ExitCanvas);
        }

        public void EnterCanvas() {
            // TODO: 3 is default count, drive it from somewhere else like session manager
            var offers = OfferingManager.Offer(3);
            for (var i = 0; i < offers.Length; i++) {
                _spritePool[i].SetUp(offers[i]);
                _spritePool[i].Enable();
            }

            _thisCanvas.enabled = true;
        }

        public void ExitCanvas() {
            foreach (var sprite in _spritePool) sprite.DisableInteractions();
            // After canvas is hidden:
            foreach (var sprite in _spritePool) {
                sprite.Clear();
                sprite.Disable();
            }

            _thisCanvas.enabled = false;
        }
    }
}