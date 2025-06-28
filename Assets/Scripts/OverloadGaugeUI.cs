using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EmergencyRoulette
{
    public class OverloadGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image gaugeFillImage;

        private float currentFill = 0f;
        private float speed = 3f; // 이 값이 클수록 빨리 반응함

        void Update()
        {
            float targetFill = Mathf.Clamp01(GameManager.Instance.PlayerState.OverloadGauge / 100f);
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * speed);
            gaugeFillImage.fillAmount = currentFill;
        }
    }
}