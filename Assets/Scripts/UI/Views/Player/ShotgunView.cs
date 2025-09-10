using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Views.Player {
    public class ShotgunView : CanvasView {
        [SerializeField] private TMP_Text _maxAmmoCount;
        [SerializeField] private TMP_Text _currentAmmoCount;
        
        [SerializeField] private Color _activeAmmoColor = Color.white;
        [SerializeField] private Color _inactiveAmmoColor = new Color(255, 255, 255, 0.33f);
        
        [SerializeField] private Color _fadeInColor;
        [SerializeField] private Vector2 _fadeInOutDuration;
        private Sequence _activeTween;
        
        public void SetCurrentAmmoCount(int value) => _currentAmmoCount.text = value.ToString();
        public void SetMaxAmmoCount(int value) => _maxAmmoCount.text = value.ToString();

        public void SetCurrentAmmoCountActive(bool isActive) {
            if (_activeTween is { active: true}) _activeTween.Complete();
            _currentAmmoCount.color = isActive ? _activeAmmoColor : _inactiveAmmoColor;
        }

        public void HighlightAmmoCount() {
            if (_activeTween is { active: true }) _activeTween.Complete(); 
            
            _activeTween = ChangeColor(_currentAmmoCount, _fadeInColor, _fadeInOutDuration);
        }

        private Sequence ChangeColor(TMP_Text text, Color color, Vector2 fadeInOut) {
            var originalColor = text.color;

            var tween = DOTween.Sequence();
            tween.Append(text.DOColor(color, fadeInOut.x));
            tween.Append(text.DOColor(originalColor, fadeInOut.y));
            tween.Play();

            return tween;
        }
    }
}