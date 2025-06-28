using UnityEngine;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using AssetKits.ParticleImage.Enumerations;

namespace EmergencyRoulette
{
    public class SlotParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleImage particleImage;

        [SerializeField] private Sprite foodSprite;
        [SerializeField] private Sprite dataSprite;
        [SerializeField] private Sprite energySprite;
        [SerializeField] private Sprite medicalSprite;

        [SerializeField] private Transform foodAttractor;
        [SerializeField] private Transform dataAttractor;
        [SerializeField] private Transform energyAttractor;
        [SerializeField] private Transform medicalAttractor;

        public void Play()
        {
            SymbolType type = SymbolType.Food;
            int emissionRatePerSecond = 20;

            Sprite selectedSprite = null;
            Transform selectedAttractor = null;

            switch (type)
            {
                case SymbolType.Food:
                    selectedSprite = foodSprite;
                    selectedAttractor = foodAttractor;
                    break;
                case SymbolType.Data:
                    selectedSprite = dataSprite;
                    selectedAttractor = dataAttractor;
                    break;
                case SymbolType.Energy:
                    selectedSprite = energySprite;
                    selectedAttractor = energyAttractor;
                    break;
                case SymbolType.Medical:
                    selectedSprite = medicalSprite;
                    selectedAttractor = medicalAttractor;
                    break;
                default:
                    Debug.LogWarning($"[SlotParticleEffect] No effect for symbol: {type}");
                    return;
            }

            // 진짜 작동하는 필드 이름은 소문자 image / attractor / emissionRate
            particleImage.sprite = selectedSprite;
            particleImage.attractorTarget = selectedAttractor;
            particleImage.rateOverTime = emissionRatePerSecond * 2;

            // 실행
            particleImage.Play();
        }
    }
}
