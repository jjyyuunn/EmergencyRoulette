using UnityEngine;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using AssetKits.ParticleImage.Enumerations;
using System.Collections;
using static UnityEngine.ParticleSystem;
using Unity.Mathematics;

namespace EmergencyRoulette
{
    public class SlotParticleEffect : MonoBehaviour
    {
        [SerializeField] private GameObject particleImageObject;
        [SerializeField] private ParticleImage particleImage;

        [SerializeField] private Sprite energySprite;
        [SerializeField] private Sprite foodSprite;
        [SerializeField] private Sprite technologySprite;
        [SerializeField] private Sprite dataSprite;

        [SerializeField] private Transform energyAttractor;
        [SerializeField] private Transform foodAttractor;
        [SerializeField] private Transform technologyAttractor;
        [SerializeField] private Transform dataAttractor;

        private SymbolType pendingType;
        private bool hasPendingType = false;

        public void SetPending(SymbolType type)
        {
            if(type == SymbolType.Warning || type == SymbolType.Discharge || type == SymbolType.Outdated)
            {
                hasPendingType = false;
            }
            else
            {
                pendingType = type;
                hasPendingType = true;
            }
        }

        public void SetupAttractors(Transform energy, Transform food, Transform tech, Transform data)
        {
            energyAttractor = energy;
            foodAttractor = food;
            technologyAttractor = tech;
            dataAttractor = data;
        }


        public void PlayPending()
        {
            if (!hasPendingType)
                return;

            Play();
            hasPendingType = false;
        }

        public void Play()
        {
            StartCoroutine(PlayRoutine());
        }


        private IEnumerator PlayRoutine()
        {
            particleImageObject.SetActive(true);

            Sprite selectedSprite = null;
            Transform selectedAttractor = null;

            Color selectedColor = Color.white;

            switch (pendingType)
            {
                case SymbolType.Energy:
                    selectedSprite = energySprite;
                    selectedAttractor = energyAttractor;
                    ColorUtility.TryParseHtmlString("#DFFF50", out selectedColor);
                    break;
                case SymbolType.Food:
                    selectedSprite = foodSprite;
                    selectedAttractor = foodAttractor;
                    ColorUtility.TryParseHtmlString("#50DB1D", out selectedColor);
                    break;
                case SymbolType.Technology:
                    selectedSprite = technologySprite;
                    selectedAttractor = technologyAttractor;
                    ColorUtility.TryParseHtmlString("#95EDFF", out selectedColor);
                    break;
                case SymbolType.Data:
                    selectedSprite = dataSprite;
                    selectedAttractor = dataAttractor;
                    ColorUtility.TryParseHtmlString("#FFAB25", out selectedColor);
                    break;
                default:
                    Debug.LogWarning($"[SlotParticleEffect] No effect for symbol: {pendingType}");
                    yield break;
            }

            particleImage.sprite = selectedSprite;
            particleImage.attractorTarget = selectedAttractor;
            particleImage.startColor = selectedColor;

            particleImage.Play();

            yield return new WaitForSeconds(2f);

            particleImageObject.SetActive(false);
        }
    }
}
