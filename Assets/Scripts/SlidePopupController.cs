using UnityEngine;
using DG.Tweening;

public class SlidePopupController : MonoBehaviour
{
    public float slideDistance = 800f;      // 오른쪽으로 얼마나 이동할지
    public float slideDuration = 0.4f;      // 애니메이션 시간

    private bool isOpen = false;
    private Vector2 originalPos;
    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalPos = rt.anchoredPosition; // 시작 위치 저장 (화면 왼쪽 바깥이라고 가정)
    }

    public void TogglePopup()
    {
        if (isOpen)
        {
            rt.DOAnchorPos(originalPos, slideDuration);
            isOpen = false;
        }
        else
        {
            rt.DOAnchorPos(originalPos + new Vector2(slideDistance, 0), slideDuration);
            isOpen = true;
        }
    }
}