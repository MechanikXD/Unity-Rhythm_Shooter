using DG.Tweening;
using JetBrains.Annotations;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.Managers {
    public class Beat {
        private Image _image;
        private Transform _transform;
        private bool _isMirrored;
        [CanBeNull] private BeatSettings _settings;
        private Tweener _animation;

        public Image AttachedImage => _image;

        public void SetSettings(BeatSettings settings) {
            if (_animation != null && _animation.IsActive()) {
                _image.color = settings.MainColor;
            }
            
            _settings = settings;
        }

        public void InstantiateSelf(Image beatPrefab, RectTransform parent, bool isMirrored) {
            var gameObj = Object.Instantiate(beatPrefab, parent, false);
            _image = gameObj;
            _transform = gameObj.transform;
            _isMirrored = isMirrored;

            if (_isMirrored) _transform.localScale = new Vector3(-1, -1, 1);
        }

        public void SetDefaultState(BeatSettings settings) {
            settings.MoveToDefaultState(_transform, _image, _isMirrored);
            _animation = null;
            _settings = null;
        }
        
        public void Animate(BeatSettings settings, float animationTime) {
            if (_settings != null) {
                _animation = _settings.Animate(_transform, _image, animationTime, _isMirrored)
                    .OnComplete(() => SetDefaultState(_settings));
            }
            else {
                _animation = settings.Animate(_transform, _image, animationTime, _isMirrored)
                    .OnComplete(() => SetDefaultState(settings));
            }
        }
    }
}