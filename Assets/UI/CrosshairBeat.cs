using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CrosshairBeat : MonoBehaviour {
        [SerializeField] private GameObject beatSprite;

        [SerializeField] private RectTransform leftBeatArea;
        [SerializeField] private RectTransform rightBeatArea;
        
        [SerializeField] private float beatAreaWidth = 300f;
        [SerializeField] private float singleBeatTime = 1f;
        [SerializeField] private float fadeInOutTime = 0.1f;

        private readonly Color _transparentWhite = new Color(255, 255, 255, 0);

        private void Start() {
            StartCoroutine(SpawnEachSecond());
        }

        private void Spawn() {
            // Spawn left beat
            var newLeftBeat = Instantiate(beatSprite, leftBeatArea, false);
            newLeftBeat.transform.localPosition = new Vector3(-beatAreaWidth / 2, 0, 0);
            // Animate: Fade in -> move to crosshair -> fade out
            var newLeftBeatImage = newLeftBeat.GetComponent<Image>();
            newLeftBeatImage.DOColor(Color.white, fadeInOutTime);
            newLeftBeat.transform.DOLocalMove(new Vector3(beatAreaWidth / 2, 0, 0), singleBeatTime)
                .OnComplete(() => newLeftBeatImage.DOColor(_transparentWhite, fadeInOutTime).OnComplete(() => Destroy(newLeftBeat)));
            // Spawn right beat
            var newRightBeat = Instantiate(beatSprite, rightBeatArea, false);
            var rightBeatTransform = newRightBeat.transform;
            rightBeatTransform.localScale = new Vector3(-1, -1, 1);
            rightBeatTransform.localPosition = new Vector3(beatAreaWidth / 2, 0, 0);
            // Animate: Fade in -> move to crosshair -> fade out
            var newRightBeatImage = newRightBeat.GetComponent<Image>();
            newRightBeatImage.DOColor(Color.white, fadeInOutTime);
            newRightBeat.transform.DOLocalMove(new Vector3(-beatAreaWidth / 2, 0, 0), singleBeatTime)
                .OnComplete(() => newRightBeatImage.DOColor(_transparentWhite, fadeInOutTime).OnComplete(() => Destroy(newRightBeat)));
        }

        private IEnumerator SpawnEachSecond() {
            while (true) {
                Spawn();
                yield return new WaitForSeconds(60f/100f);
            }
        }
    }
}