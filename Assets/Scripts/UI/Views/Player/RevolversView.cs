using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Views.Player {
    public class RevolversView : CanvasView {
        [SerializeField] private TMP_Text _leftAmmoCount;
        [SerializeField] private TMP_Text _rightAmmoCount;
        [SerializeField] private TMP_Text _maxAmmoCount;

        [SerializeField] private Color _fadeInColor;
        [SerializeField] private Vector2 _fadeInOutDuration;
        private Sequence _activeTween;

        public void SetLeftAmmoCount(int value) => _leftAmmoCount.text = value.ToString();
        
        public void SetRightAmmoCount(int value) => _rightAmmoCount.text = value.ToString();
        
        public void SetMaxAmmoCount(int value) => _maxAmmoCount.text = value.ToString();

        public void HighlightLeftAmmoCount() {
            if (_activeTween is { active: true }) _activeTween.Complete(); 
            
            ChangeColor(_leftAmmoCount, _fadeInColor, _fadeInOutDuration);
        }

        public void HighlightRightAmmoCount() {
            if (_activeTween is { active: true }) _activeTween.Complete(); 
            
            _activeTween = ChangeColor(_rightAmmoCount, _fadeInColor, _fadeInOutDuration);
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