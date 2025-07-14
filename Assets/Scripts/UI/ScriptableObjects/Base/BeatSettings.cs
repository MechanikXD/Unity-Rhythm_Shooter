using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScriptableObjects.Base {
    [CreateAssetMenu(fileName = "BeatSettings", menuName = "Scriptable Objects/BeatSettings")]
    public class BeatSettings : ScriptableObject {
        [SerializeField] private Vector2 fadeInOutTime = new Vector2(0.1f, 0.1f);
        [SerializeField] private Vector2 startEndPosition = new Vector2(-150f, 150f);
        [SerializeField] private Color beatColor = Color.white;

        public Color MainColor => beatColor;

        public void MoveToDefaultState(Transform attachedTransform,
            Image attachedImage, bool mirrorAnimation) {
            attachedImage.color = new Color(beatColor.r, beatColor.g, beatColor.b, 0); // Transparent
            attachedTransform.localPosition =
                new Vector3(mirrorAnimation ? startEndPosition.y : startEndPosition.x, 0, 0); // To origin
        }

        public TweenerCore<Vector3, Vector3, VectorOptions> Animate(Transform attachedTransform,
            Image attachedImage, float travelTime, bool mirrorAnimation) {
            // Animations
            attachedImage.DOColor(beatColor, fadeInOutTime.x); // Fate in
            var animation =
                attachedTransform.DOLocalMove(
                    new Vector3(mirrorAnimation ? startEndPosition.x : startEndPosition.y, 0, 0),
                    travelTime);
            animation.SetEase(Ease.Linear); // Move to destination
            animation.OnComplete(() =>
                attachedImage.DOColor(new Color(beatColor.r, beatColor.g, beatColor.b, 0),
                    fadeInOutTime.y)); // Fate out
            return animation;
        }
    }
}