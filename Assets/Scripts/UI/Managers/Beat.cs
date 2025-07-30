using DG.Tweening;
using JetBrains.Annotations;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.Managers {
    /// <summary>
    /// Data holder for beats that appear on screen around crosshair.
    /// </summary>
    public class Beat {
        private Image _image;
        private Transform _transform;
        private bool _isMirrored;
        [CanBeNull] private BeatSettings _settings;
        private Tweener _animation;

        public Tweener CurrentAnimation => _animation;
        
        /// <summary>
        /// Sets new settings for current instance of beat. if in progress only changes it's color. 
        /// </summary>
        public void SetNewSettings(BeatSettings settings) {
            if (_animation != null && _animation.IsActive()) {
                _image.color = settings.MainColor;
            }
            
            _settings = settings;
        }
        
        // Creates new instance of Beat
        public void InstantiateSelf(Image beatPrefab, RectTransform parent, bool isMirrored) {
            var gameObj = Object.Instantiate(beatPrefab, parent, false);
            _image = gameObj;
            _transform = gameObj.transform;
            _isMirrored = isMirrored;

            if (_isMirrored) _transform.localScale = new Vector3(-1, -1, 1);
        }
        
        /// <summary>
        /// Moves beat to starting position, from where it will be further animated.
        /// Use this method before <b>Animate</b> to set up animation correctly.
        /// </summary>
        public void SetDefaultState(BeatSettings settings) {
            settings.MoveToDefaultState(_transform, _image, _isMirrored);
            _animation = null;
            _settings = null;
        }

        /// <summary>
        /// Animates this instance with given BeatSettings animation function.
        /// Use <b>SetDefaultState</b> method before calling this one to properly set up animation.
        /// Sets instance to default position after completion.
        /// </summary>
        /// <param name="settings"> Setting that animated and  </param>
        /// <param name="animationTime"> Duration of beat movement </param>
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