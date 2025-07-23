using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Score {
    [Serializable]
    public class RankView {
        [SerializeField] private Image _fillImage;
        [SerializeField] private GameObject _mainObject;

        public void ChangeFill(float fillAmount) => _fillImage.fillAmount = fillAmount;

        public void Enable() => _mainObject.SetActive(true);
        public void Disable() => _mainObject.SetActive(false);
    }
}