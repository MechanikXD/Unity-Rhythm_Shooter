using System.Collections;
using DG.Tweening;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class CrosshairBeat : MonoBehaviour {
        [SerializeField] private GameObject beatPrefab;
        
        [SerializeField] private BeatInstance normalBeatObjectLeft;
        [SerializeField] private BeatInstance normalBeatObjectRight;

        [SerializeField] private RectTransform leftBeatArea;
        [SerializeField] private RectTransform rightBeatArea;
        
        [SerializeField] private float singleBeatTime = 1f;

        private void Start() {
            // This will skip first beat
            /*singleBeatTime = Conductor.Instance.SongData.Crotchet;
            ConductorEvents.NextBeatEvent += Spawn;*/

            StartCoroutine(SpawnEachHalfSeconds());
        }

        private (GameObject leftBeat, GameObject rightBeat) InstantiateNewBeats() {
            var newLeftBeat = Instantiate(beatPrefab, leftBeatArea, false);
            normalBeatObjectLeft.SetupSelf(newLeftBeat.transform, newLeftBeat.GetComponent<Image>());
            
            var newRightBeat = Instantiate(beatPrefab, rightBeatArea, false);
            var rightBeatTransform = newRightBeat.transform;
            rightBeatTransform.localScale = new Vector3(-1, -1, 1);
            normalBeatObjectRight.SetupSelf(rightBeatTransform, newRightBeat.GetComponent<Image>());

            return (newLeftBeat, newRightBeat);
        }

        private void Spawn() {
            var newBeats = InstantiateNewBeats();
            normalBeatObjectLeft.Animate(singleBeatTime).OnComplete(() => Destroy(newBeats.leftBeat));
            normalBeatObjectRight.Animate(singleBeatTime).OnComplete(() => Destroy(newBeats.rightBeat));
        }

        private IEnumerator SpawnEachHalfSeconds() {
            while (true) {
                Spawn();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}