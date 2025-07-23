using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScriptableObjects.Base {
    [CreateAssetMenu(fileName = "BeatSettings", menuName = "Scriptable Objects/BeatSettings")]
    public class BeatSettings : ScriptableObject {
        [SerializeField] private Vector2 _fadeInOutTime = new Vector2(0.1f, 0.1f);
        [SerializeField] private Vector2 _startEndPosition = new Vector2(-150f, 150f);
        [SerializeField] private Color _beatColor = Color.white;

        public Color MainColor => _beatColor;

        public void MoveToDefaultState(Transform attachedTransform,
            Image attachedImage, bool mirrorAnimation) {
            attachedImage.color = new Color(_beatColor.r, _beatColor.g, _beatColor.b, 0); // Transparent
            attachedTransform.localPosition =
                new Vector3(mirrorAnimation ? _startEndPosition.y : _startEndPosition.x, 0, 0); // To origin
        }

        public TweenerCore<Vector3, Vector3, VectorOptions> Animate(Transform attachedTransform,
            Image attachedImage, float travelTime, bool mirrorAnimation) {
            // Animations
            attachedImage.DOColor(_beatColor, _fadeInOutTime.x); // Fate in
            var animation =
                attachedTransform.DOLocalMove(
                    new Vector3(mirrorAnimation ? _startEndPosition.x : _startEndPosition.y, 0, 0),
                    travelTime);
            animation.SetEase(Ease.Linear); // Move to destination
            animation.OnComplete(() =>
                attachedImage.DOColor(new Color(_beatColor.r, _beatColor.g, _beatColor.b, 0),
                    _fadeInOutTime.y)); // Fate out
            return animation;
        }
    }
}