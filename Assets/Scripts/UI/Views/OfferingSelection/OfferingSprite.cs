using Core.Offerings;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Views.OfferingSelection {
    public class OfferingSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IPointerClickHandler {
        private bool _canInteractWith;
        [SerializeField] private float _moveDistance;
        [SerializeField] private float _moveDuration;
        private float _originalPosition;
        private float _endValue;
        [SerializeField] private Image _art;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        private OfferingBase _offering;
        private Tweener _currentMovement;

        private void Awake() {
            _originalPosition = transform.position.y;
            _endValue = _originalPosition + _moveDistance;
        }

        public void SetUp(OfferingBase offering) {
            _offering = offering;
            _art = offering.Art;
            _title.SetText(offering.Title);
            _description.SetText(offering.Description);
        }

        public void Clear() {
            _offering = null;
            _art = null;
            _title.SetText("");
            _description.SetText("");
            _canInteractWith = false;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!_canInteractWith) return;
            OfferingManager.SelectOffering(_offering);
            UIManager.Instance.ExitLastCanvas();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (_currentMovement != null && _currentMovement.IsPlaying()) _currentMovement.Pause();
            _currentMovement = gameObject.transform.DOMoveY(_endValue, _moveDuration)
                .SetUpdate(true);
            _currentMovement.Play();
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (_currentMovement != null && _currentMovement.IsPlaying()) _currentMovement.Pause();
            _currentMovement = gameObject.transform.DOMoveY(_originalPosition, _moveDuration)
                .SetUpdate(true);
            _currentMovement.Play();
        }

        public void Enable() {
            gameObject.SetActive(true);
            _canInteractWith = true;
        }

        public void Disable() {
            gameObject.SetActive(false);
            _currentMovement?.Kill();
        }

        public void DisableInteractions() => _canInteractWith = false;
    }
}