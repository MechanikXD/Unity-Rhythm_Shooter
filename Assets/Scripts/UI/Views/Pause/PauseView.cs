using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Views.Pause {
    public class PauseView : CanvasView {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _exitButton;

        private void OnEnable() {
            _resumeButton.onClick.AddListener(UIManager.Instance.ExitLastCanvas);
            _giveUpButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
            _exitButton.onClick.AddListener(Application.Quit);
        }

        private void OnDisable() {
            _resumeButton.onClick.RemoveListener(UIManager.Instance.ExitLastCanvas);
            _giveUpButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveListener(Application.Quit);
        }
    }
}