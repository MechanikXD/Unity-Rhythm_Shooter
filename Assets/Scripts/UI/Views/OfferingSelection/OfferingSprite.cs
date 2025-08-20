using Core.Offerings;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.OfferingSelection {
    public class OfferingSprite : MonoBehaviour {
        private bool _canInteractWith;
        [SerializeField] private float _moveDistance;
        [SerializeField] private float _moveDuration;
        private Vector2 _originalPosition;
        [SerializeField] private Image _art;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        private OfferingBase _offering;
        private Tweener _currentMovement;

        private void Awake() {
            _originalPosition = transform.position;
        }

        public void SetUp(OfferingBase offering) {
            _offering = offering;
            _art = offering.Art;
            _title.SetText(offering.Title);
            _description.SetText(offering.Description);
            _canInteractWith = true;
        }

        public void Clear() {
            _offering = null;
            _art = null;
            _title.SetText("");
            _description.SetText("");
            _canInteractWith = false;
        }

        private void OnMouseDown() {
            if (!_canInteractWith) return;
            OfferingManager.SelectOffering(_offering);
            UIManager.Instance.ExitLastCanvas();
        }

        private void OnMouseEnter() {
            if (_currentMovement != null && _currentMovement.IsPlaying()) _currentMovement.Pause();
            _currentMovement = gameObject.transform.DOMoveY(_moveDistance, _moveDuration)
                .SetUpdate(true);
        }

        private void OnMouseExit() {
            if (_currentMovement != null && _currentMovement.IsPlaying()) _currentMovement.Pause();
            _currentMovement = gameObject.transform.DOMoveY(_originalPosition.y, _moveDuration)
                .SetUpdate(true);
        }

        public void Enable() {
            gameObject.SetActive(true);
        }

        public void Disable() {
            gameObject.SetActive(false);
            _currentMovement?.Kill();
        }

        public void DisableInteractions() => _canInteractWith = false;
    }
}