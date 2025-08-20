using Core.Offerings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.OfferingSelection {
    public class OfferingView : CanvasView {
        [SerializeField] private OfferingSprite[] _spritePool;
        [SerializeField] private Button _forgetButton;

        private void OnEnable() {
            _forgetButton.onClick.AddListener(UIManager.Instance.ExitLastCanvas);
        }

        private void OnDisable() {
            _forgetButton.onClick.RemoveListener(UIManager.Instance.ExitLastCanvas);
        }

        public override void EnterCanvas() {
            // TODO: 3 is default count, drive it from somewhere else like session manager
            var offers = OfferingManager.Offer(3);
            for (var i = 0; i < offers.Length; i++) {
                _spritePool[i].SetUp(offers[i]);
                _spritePool[i].Enable();
            }

            base.EnterCanvas();
        }

        public override void ExitCanvas() {
            foreach (var sprite in _spritePool) sprite.DisableInteractions();
            // After canvas is hidden:
            foreach (var sprite in _spritePool) {
                sprite.Clear();
                sprite.Disable();
            }

            base.ExitCanvas();
        }
    }
}