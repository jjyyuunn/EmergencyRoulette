using UnityEngine;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using AssetKits.ParticleImage.Enumerations;
using System.Collections;

namespace EmergencyRoulette
{
    public class SlotParticleEffect : MonoBehaviour
    {
        [SerializeField] private GameObject particleImageObject;
        [SerializeField] private ParticleImage particleImage;

        [SerializeField] private Sprite energySprite;
        [SerializeField] private Sprite foodSprite;
        [SerializeField] private Sprite medicalSprite;
        [SerializeField] private Sprite dataSprite;

        [SerializeField] private Transform energyAttractor;
        [SerializeField] private Transform foodAttractor;
        [SerializeField] private Transform medicalAttractor;
        [SerializeField] private Transform dataAttractor;

        public void Play()
        {
            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            particleImageObject.SetActive(true);

            SymbolType type = SymbolType.Food;
            int emissionRatePerSecond = 20;

            Sprite selectedSprite = null;
            Transform selectedAttractor = null;

            switch (type)
            {
                case SymbolType.Energy:
                    selectedSprite = energySprite;
                    selectedAttractor = energyAttractor;
                    break;
                case SymbolType.Food:
                    selectedSprite = foodSprite;
                    selectedAttractor = foodAttractor;
                    break;
                case SymbolType.Medical:
                    selectedSprite = medicalSprite;
                    selectedAttractor = medicalAttractor;
                    break;
                case SymbolType.Data:
                    selectedSprite = dataSprite;
                    selectedAttractor = dataAttractor;
                    break;
                default:
                    Debug.LogWarning($"[SlotParticleEffect] No effect for symbol: {type}");
                    yield break;
            }

            particleImage.sprite = selectedSprite;
            particleImage.attractorTarget = selectedAttractor;
            particleImage.rateOverTime = emissionRatePerSecond * 2;

            particleImage.Play();

            yield return new WaitForSeconds(2f);

            particleImageObject.SetActive(false);
        }
    }
}
