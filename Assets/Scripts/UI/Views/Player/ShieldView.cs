using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Player {
    public class ShieldView : CanvasView {
        [SerializeField] private Image _shieldIcon;
        
        [SerializeField] private Color _activeIconColor = Color.white;
        [SerializeField] private Color _inactiveColor = new Color(255, 255, 255, 0.5f);

        public void SetShieldIconActive(bool isActive) {
            _shieldIcon.color = isActive ? _activeIconColor : _inactiveColor;
        }
    }
}