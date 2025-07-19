using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Score {
    [Serializable]
    public class RankView {
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject mainObject;

        public void ChangeFill(float fillAmount) => fillImage.fillAmount = fillAmount;

        public void Enable() => mainObject.SetActive(true);
        public void Disable() => mainObject.SetActive(false);
    }
}