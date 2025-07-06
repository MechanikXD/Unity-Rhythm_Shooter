using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScriptableObjects.Base {
    [CreateAssetMenu(fileName = "BeatInstance", menuName = "Scriptable Objects/BeatInstance")]
    public class BeatInstance : ScriptableObject {
        [SerializeField] private Vector2 fadeInOutTime = new Vector2(0.1f, 0.1f);
        [SerializeField] private Vector2 startEndPosition = new Vector2(-150f, 150f);
        [SerializeField] private Color beatColor = Color.white;

        private Color _selfTransparentColor;
        private Transform _attachedTransform;
        private Image _attachedImage;

        public void SetupSelf(Transform attachedTransform, Image attachedImage) {
            _attachedTransform = attachedTransform;
            _attachedImage = attachedImage;
            // Make it transparent
            _selfTransparentColor = new Color(beatColor.r, beatColor.g, beatColor.b, 0);
            attachedImage.color = _selfTransparentColor;
            attachedTransform.localPosition = new Vector3(startEndPosition.x, 0, 0);
        }

        public TweenerCore<Vector3, Vector3, VectorOptions> Animate(float travelTime) {
             // Fate in
            _attachedImage.DOColor(beatColor, fadeInOutTime.x);
            // Move to destination
            var move = _attachedTransform.DOLocalMove(new Vector3(startEndPosition.y, 0, 0), travelTime);
            move.SetEase(Ease.Linear);
            // Fate out
            move.OnComplete(() => _attachedImage.DOColor(_selfTransparentColor, fadeInOutTime.y));
            return move;
        }
    }
}