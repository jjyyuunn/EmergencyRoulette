using UnityEngine;
using TMPro;
using DG.Tweening;

public class SlotParticleTextTester : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private RectTransform iconImage;

    private int currentCount = 0;
    private Transform textTransform;

    private void Start()
    {
        textTransform = countText.transform;
        countText.text = currentCount.ToString();
    }

    public void AddOne()
    {
        currentCount++;
        countText.text = currentCount.ToString();

        // �ؽ�Ʈ Ʈ��
        textTransform.DOKill();
        textTransform.localScale = Vector3.one;

        textTransform
            .DOScale(1.25f, 0.025f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                textTransform.DOScale(1f, 0.025f).SetEase(Ease.InQuad);
            });

        // ������ Ʈ��
        iconImage.DOKill();
        iconImage.localScale = Vector3.one;

        iconImage
            .DOScale(1.25f, 0.025f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                iconImage.DOScale(1f, 0.025f).SetEase(Ease.InQuad);
            });
    }
}
