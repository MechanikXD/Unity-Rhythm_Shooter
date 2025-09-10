using Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Player {
    public class PlayerView : CanvasView {
        [SerializeField] private RectTransform _healthBar; 
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _dashIconFill;
        [SerializeField] private int _unitPerHealthPoint = 3;
        
        private int _playerMaxHealth;

        private void Start() {
            var player = GameManager.Instance.Player;
            _playerMaxHealth = player.MaxHealth;
            SetHealthFill(player.CurrentHealth / (float)_playerMaxHealth);
            
            SetDashIconFill(1f);
            
            GameManager.Instance.Player.MaxHealthChanged += AdjustHealthWidth;
            GameManager.Instance.Player.CurrentHealthChanged += UpdateHealthFill;
        }

        private void UpdateHealthFill(int newValue) => SetHealthFill((float)newValue / _playerMaxHealth);

        private void AdjustHealthWidth(int newValue) {
            _playerMaxHealth = newValue;
            
            var rect = _healthBar.rect;
            _healthBar.rect.Set(rect.x, rect.y, newValue * _unitPerHealthPoint, rect.height);
        }

        private void SetHealthFill(float fillAmount) {
            var finalAmount = Mathf.Clamp(fillAmount, 0, 1);
            _healthFill.fillAmount = finalAmount;
        }

        public void SetDashIconFill(float fillAmount) {
            var finalAmount = Mathf.Clamp(fillAmount, 0, 1);
            _dashIconFill.fillAmount = finalAmount;
        }
    }
}