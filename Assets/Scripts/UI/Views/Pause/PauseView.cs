using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pause {
    public class PauseView : MonoBehaviour, ICanvasView {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _exitButton;
        private Canvas _thisCanvas;

        private void OnEnable() {
            // TODO: Sub all the events here
        }

        private void Awake() {
            _thisCanvas = GetComponent<Canvas>();
        }

        private void OnDisable() {
            
        }

        public void EnterCanvas() {
            _thisCanvas.enabled = true;
        }

        public void ExitCanvas() {
            _thisCanvas.enabled = false;
        }
    }
}