using System.Collections;
using DG.Tweening;
using Player;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class CrosshairBeat : MonoBehaviour {
        [SerializeField] private GameObject beatPrefab;
        
        [SerializeField] private BeatInstance leftBeatParams;
        [SerializeField] private BeatInstance rightBeatParams;

        [SerializeField] private Image leftGradientImage;
        [SerializeField] private Image rightGradientImage;
        [SerializeField] private Vector2 gradientFadeInOutTime = new Vector2(0.05f, 0.5f);
        private Sequence _leftGradientAnimation;
        private Sequence _rightGradientAnimation;

        [SerializeField] private RectTransform leftBeatArea;
        [SerializeField] private RectTransform rightBeatArea;
        
        [SerializeField] private float singleBeatTime = 1f;

        private readonly Color _transparentWhite = new Color(255, 255, 255, 0);

        private void OnEnable() {
            PlayerEvents.LeftActionEvent += ShowLeftGradient;
            PlayerEvents.RightActionEvent += ShowRightGradient;
            PlayerEvents.BothActionsEvent += ShowLeftGradient;
            PlayerEvents.BothActionsEvent += ShowRightGradient;
        }

        private void Start() {
            // This will skip first beat
            /*singleBeatTime = Conductor.Instance.SongData.Crotchet;
            ConductorEvents.NextBeatEvent += Spawn;*/

            StartCoroutine(SpawnEachHalfSeconds());
        }

        private void OnDisable() {
            PlayerEvents.LeftActionEvent -= ShowLeftGradient;
            PlayerEvents.RightActionEvent -= ShowRightGradient;
            PlayerEvents.BothActionsEvent -= ShowLeftGradient;
            PlayerEvents.BothActionsEvent -= ShowRightGradient;
        }

        private (GameObject leftBeat, GameObject rightBeat) InstantiateNewBeats() {
            var newLeftBeat = Instantiate(beatPrefab, leftBeatArea, false);
            leftBeatParams.SetupSelf(newLeftBeat.transform, newLeftBeat.GetComponent<Image>());
            
            var newRightBeat = Instantiate(beatPrefab, rightBeatArea, false);
            var rightBeatTransform = newRightBeat.transform;
            rightBeatTransform.localScale = new Vector3(-1, -1, 1);
            rightBeatParams.SetupSelf(rightBeatTransform, newRightBeat.GetComponent<Image>());

            return (newLeftBeat, newRightBeat);
        }

        private void ShowLeftGradient() {
            if (_leftGradientAnimation is { active: true }) {
                _leftGradientAnimation.Restart();
            }
            else {
                _leftGradientAnimation = DOTween.Sequence();
                _leftGradientAnimation.Append(leftGradientImage.DOColor(Color.white,
                    gradientFadeInOutTime.x));
                _leftGradientAnimation.Append(leftGradientImage.DOColor(_transparentWhite,
                    gradientFadeInOutTime.y));
                _leftGradientAnimation.Play();
            }
        }

        private void ShowRightGradient() {
            if (_rightGradientAnimation is { active: true }) {
                _rightGradientAnimation.Restart();
            }
            else {
                _rightGradientAnimation = DOTween.Sequence();
                _rightGradientAnimation.Append(rightGradientImage
                    .DOColor(Color.white, gradientFadeInOutTime.x));
                _rightGradientAnimation.Append(
                    rightGradientImage.DOColor(_transparentWhite, gradientFadeInOutTime.y));
                _rightGradientAnimation.Play();
            }
        }

        private void Spawn() {
            var newBeats = InstantiateNewBeats();
            leftBeatParams.Animate(singleBeatTime).OnComplete(() => Destroy(newBeats.leftBeat));
            rightBeatParams.Animate(singleBeatTime).OnComplete(() => Destroy(newBeats.rightBeat));
        }

        private IEnumerator SpawnEachHalfSeconds() {
            while (true) {
                Spawn();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}