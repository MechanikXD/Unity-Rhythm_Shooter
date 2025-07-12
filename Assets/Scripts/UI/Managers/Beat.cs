using DG.Tweening;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class Beat {
        private Image _image;
        private Transform _transform;
        private bool _isMirrored;

        public Image AttachedImage => _image;

        public void InstantiateSelf(Image beatPrefab, RectTransform parent, bool isMirrored) {
            var gameObj = Object.Instantiate(beatPrefab, parent, false);
            _image = gameObj;
            _transform = gameObj.transform;
            _isMirrored = isMirrored;

            if (_isMirrored) _transform.localScale = new Vector3(-1, -1, 1);
        }

        public void SetDefaultState(BeatSettings settings) => 
            settings.MoveToDefaultState(_transform, _image, _isMirrored);

        public void Animate(BeatSettings settings, float animationTime) =>
            settings.Animate(_transform, _image, animationTime, _isMirrored)
                .OnComplete(() => SetDefaultState(settings));
    }
}