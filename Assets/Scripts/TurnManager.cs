/*

using System;
using System.Collections;
using UnityEngine;

namespace EmergencyRoulette
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public int currentTurn { get; private set; } = 1;
        public int maxTurn = 16;

        public event Action<int> OnTurnStarted;
        public event Action<int> OnTurnEnded;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void StartGame()
        {
            currentTurn = 1;
            StartCoroutine(RunTurn());
        }

        public void NextTurn()
        {
            currentTurn++;
            if (currentTurn > maxTurn)
            {
                EndGame();
            }
            else
            {
                StartCoroutine(RunTurn());
            }
        }

        private IEnumerator RunTurn()
        {
            OnTurnStarted?.Invoke(currentTurn);

            yield return StartCoroutine(SpinPhase());
            yield return StartCoroutine(ResolvePhase());
            yield return StartCoroutine(DisasterPhase());
            yield return StartCoroutine(ResourcePhase());
            yield return StartCoroutine(ForecastPhase());

            OnTurnEnded?.Invoke(currentTurn);
        }

        private IEnumerator SpinPhase()
        {
            Debug.Log("1. ���� �ܰ�");

            GameManager.Instance.CanPlayerInteract = true;

            // Spin ��ư ���� ������ ��ٸ�
            yield return new WaitUntil(() => GameManager.Instance.PlayerHasSpunThisTurn);

            GameManager.Instance.CanPlayerInteract = false;
            GameManager.Instance.PlayerHasSpunThisTurn = false;

            yield return StartCoroutine(SlotManager.Instance.StartSpinAndWait());
        }

        private IEnumerator ResolvePhase()
        {
            Debug.Log("2. ��� ó�� �ܰ�");

            // ���� ���� (���� ������ ���� Ȱ��ȭ)
            // SymbolProcessor.Instance.ApplyCombos();
            // ModuleManager.Instance.ApplyPassives();
            // SymbolProcessor.Instance.ApplyBaseSymbols();
            // EmergencyManager.Instance.CheckOverloadGauge();

            yield return null;
        }

        private IEnumerator DisasterPhase()
        {
            Debug.Log("3. �糭 �̺�Ʈ �ܰ�");

            if (currentTurn % 4 == 0)
            {
                // yield return DisasterManager.Instance.ResolveDisaster(currentTurn);
                yield return null;
            }
            else
            {
                yield return null;
            }
        }

        private IEnumerator ResourcePhase()
        {
            Debug.Log("4. �⺻ �ڿ� ��� �ܰ�");

            GameManager.Instance.CanPlayerInteract = true;

            // TODO: �÷��̾ �ڿ� �ൿ ��ĥ ������ ��ٸ��� ����
            // yield return StartCoroutine(WaitForPlayerResourceActions());

            yield return new WaitForSeconds(1f); // �ӽ� ���

            GameManager.Instance.CanPlayerInteract = false;
        }

        private IEnumerator ForecastPhase()
        {
            Debug.Log("5. �糭 ���� �ܰ�");

            // DisasterManager.Instance.UpdateForecast(currentTurn);
            yield return null;
        }

        private void EndGame()
        {
            Debug.Log("6. ���� ����");

            // GameResultManager.Instance.CheckGameResult();
        }

        private bool PlayerPressedSpinButton()
        {
            return GameManager.Instance.PlayerHasSpunThisTurn;
        }
    }
}
*/