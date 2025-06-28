using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using EmergencyRoulette;

public class SlotColumnScroller : MonoBehaviour
{
    [Header("Spin Speed Settings")]
    [SerializeField] private float symbolHeight = 100f;
    [SerializeField] private float startLoopInterval = 0.3f;
    [SerializeField] private float endLoopInterval = 0.05f;
    [SerializeField] private Vector2 accelerateDurationRange = new Vector2(0.4f, 0.6f);

    [Header("Start Bounce Animation")]
    [SerializeField] private Vector2 startBounceHeightRange = new Vector2(15f, 25f);
    [SerializeField] private Vector2 startBounceDurationRange = new Vector2(0.08f, 0.13f);

    [Header("Stop Slowdown Settings")]
    [SerializeField] private Vector2Int slowdownStepsRange = new Vector2Int(12, 18);
    [SerializeField] private float slowdownFinalInterval = 0.3f;

    [Header("Final Bounce Shake")]
    [SerializeField] private Vector2 bounceShakeStrengthRange = new Vector2(15f, 25f);
    [SerializeField] private Vector2 bounceShakeDurationRange = new Vector2(0.25f, 0.35f);
    [SerializeField] private Vector2Int bounceCountRange = new Vector2Int(3, 5);
    [SerializeField] private float bounceStrengthDecay = 0.5f;
    [SerializeField] private float bounceDurationGrow = 1.1f;

    [Header("Refs")]
    [SerializeField] private SymbolView[] symbols;
    [SerializeField] private RectTransform symbolContainer;

    [System.Serializable]
    public class SymbolView
    {
        public SymbolType symbolName;
        public RectTransform rectTransform;
    }

    private float accelerateDuration;
    private float startBounceHeight;
    private float startBounceDuration;
    private int slowdownSteps;
    private float bounceShakeStrength;
    private float bounceShakeDuration;
    private int bounceCount;

    private Coroutine spinCoroutine;
    private SymbolType targetSymbolName;

    private void Start()
    {
        InitSymbols();
    }

    private void InitSymbols()
    {
        float y_offset = (symbols.Length % 2 == 0) ? symbols.Length * symbolHeight / 2f : (symbols.Length - 1) * symbolHeight / 2f;
        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i].rectTransform.anchoredPosition = new Vector2(0, -symbolHeight * i + y_offset);
        }
        symbolContainer.anchoredPosition = Vector2.zero;
    }

    public IEnumerator StartSpin(SymbolType symbol, Action onComplete)
    {
        SetRandomParams();

        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);

        symbolContainer.anchoredPosition = Vector2.zero;

        symbolContainer.DOAnchorPosY(startBounceHeight, startBounceDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                symbolContainer.DOAnchorPosY(0f, startBounceDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        spinCoroutine = StartCoroutine(SpinRoutine());
                    });
            });
        yield return new WaitForSeconds(1f);
        StopSpin(symbol);
        onComplete?.Invoke();
    }

    private void SetRandomParams()
    {
        accelerateDuration = UnityEngine.Random.Range(accelerateDurationRange.x, accelerateDurationRange.y);
        startBounceHeight = UnityEngine.Random.Range(startBounceHeightRange.x, startBounceHeightRange.y);
        startBounceDuration = UnityEngine.Random.Range(startBounceDurationRange.x, startBounceDurationRange.y);
        slowdownSteps = UnityEngine.Random.Range(slowdownStepsRange.x, slowdownStepsRange.y + 1);
        bounceShakeStrength = UnityEngine.Random.Range(bounceShakeStrengthRange.x, bounceShakeStrengthRange.y);
        bounceShakeDuration = UnityEngine.Random.Range(bounceShakeDurationRange.x, bounceShakeDurationRange.y);
        bounceCount = UnityEngine.Random.Range(bounceCountRange.x, bounceCountRange.y + 1);
    }

    private IEnumerator SpinRoutine()
    {
        symbolContainer.anchoredPosition = Vector2.zero;
        float timer = 0f;

        while (true)
        {
            float t = Mathf.Clamp01(timer / accelerateDuration);
            float currentInterval = Mathf.Lerp(startLoopInterval, endLoopInterval, t);

            bool done = false;
            Tween moveTween = symbolContainer.DOAnchorPosY(-symbolHeight, currentInterval)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    symbolContainer.anchoredPosition = Vector2.zero;
                    OnStepComplete();
                    done = true;
                });

            yield return new WaitUntil(() => done);

            timer += currentInterval;
        }
    }

    private void StopSpin(SymbolType targetSymbol)
    {
        targetSymbolName = targetSymbol;

        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
        }

        DOTween.Kill(symbolContainer);
        symbolContainer.anchoredPosition = Vector2.zero;

        spinCoroutine = StartCoroutine(SlowdownAndStopRoutine());
    }

    private IEnumerator SlowdownAndStopRoutine()
    {
        symbolContainer.anchoredPosition = Vector2.zero;

        int centerIndex = symbols.Length / 2;
        int stepsTaken = 0;
        float totalDuration = 1.5f;
        float accumulated = 0f;

        while (true)
        {
            // �� �����Ӹ��� ���� target �ε����� �ٽ� ���
            int currentIndex = Array.FindIndex(symbols, s => s.symbolName == targetSymbolName);
            if (currentIndex == -1)
            {
                Debug.LogWarning("Target symbol not found!");
                yield break;
            }

            int diff = (centerIndex - currentIndex + symbols.Length) % symbols.Length;

            // �߾ӿ��� ���� �¾����� ����
            if (diff == 0 && stepsTaken >= slowdownSteps)
                break;

            float t = Mathf.Clamp01(accumulated / totalDuration);
            float interval = Mathf.Lerp(endLoopInterval, slowdownFinalInterval, t);

            bool done = false;
            symbolContainer.DOAnchorPosY(-symbolHeight, interval)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    symbolContainer.anchoredPosition = Vector2.zero;
                    OnStepComplete();
                    done = true;
                });

            yield return new WaitUntil(() => done);
            accumulated += interval;
            stepsTaken++;
        }

        yield return StartCoroutine(PlayBounceShake());
        symbolContainer.anchoredPosition = Vector2.zero;
    }




    private IEnumerator PlayBounceShake()
    {
        float strength = bounceShakeStrength;
        float duration = bounceShakeDuration;

        for (int i = 0; i < bounceCount; i++)
        {
            yield return symbolContainer.DOAnchorPosY(-strength, duration).SetEase(Ease.OutQuad).WaitForCompletion();
            yield return symbolContainer.DOAnchorPosY(strength, duration).SetEase(Ease.OutQuad).WaitForCompletion();

            strength *= bounceStrengthDecay;
            duration *= bounceDurationGrow;
        }

        yield return symbolContainer.DOAnchorPosY(0f, 0.05f).SetEase(Ease.InQuad).WaitForCompletion();
    }

    private void OnStepComplete()
    {
        symbolContainer.anchoredPosition = Vector2.zero;

        SymbolView last = symbols[symbols.Length - 1];
        for (int i = symbols.Length - 1; i > 0; i--)
        {
            symbols[i] = symbols[i - 1];
        }
        symbols[0] = last;

        float y_offset = (symbols.Length%2 == 0) ? symbols.Length * symbolHeight / 2f : (symbols.Length-1) * symbolHeight / 2f;
        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i].rectTransform.anchoredPosition = new Vector2(0, -symbolHeight * i + y_offset);
        }
    }
}
