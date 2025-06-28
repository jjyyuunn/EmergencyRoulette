using Unity.Burst.Intrinsics;
using UnityEngine;

namespace EmergencyRoulette
{
    public static class ModuleEffectExecutor
    {
        public static void ApplyAllPassiveModules(PlayerState player)
        {
            foreach (var equip in ModuleManager.Instance.equippedModules)
            {
                var module = equip.module;
                if (module.useType != ModuleUseType.Passive) continue;

                switch (module.effectKey)
                {
                    case "AddDataPerTurn":
                        player.Data += 1;
                        Debug.Log($"[PassiveEffect] 데이터 +1");
                        break;

                    case "AddFoodPerTurn":
                        player.Food += 1;
                        Debug.Log($"[PassiveEffect] 식량 +1");
                        break;

                    case "AddTechPerTurn":
                        player.Technology += 1;
                        Debug.Log($"[PassiveEffect] 기술 +1");
                        break;

                    case "AddEnergyPerTurn":
                        player.Energy += 1;
                        Debug.Log($"[PassiveEffect] 에너지 +1");
                        break;

                    case "IgnoreWarningWithEnergy":
                        // playerState 내 존재
                        Debug.Log("[PassiveEffect] 경고 무효화");
                        break;

                    case "GainDataIfMultipleDanger":
                        // playerState 내 존재
                        Debug.Log("[PassiveEffect] 위험 2개 이상 시 데이터 +2");
                        break;

                    default:
                        Debug.LogWarning($"[PassiveEffect] Unknown key: {module.effectKey}");
                        break;
                }
            }
        }


        public static void ApplyActiveModuleAt(int index)
        {
            var state = GameManager.Instance.PlayerState;
            var module = ModuleManager.Instance.GetEquippedModule(index);
            if (module == null || module.useType != ModuleUseType.Active)
                return;

            // 에너지 부족 등은 여기서 체크

            bool applyTwice = state.DoubleNextActive && module.effectKey != "DoubleNextActive";
            if (applyTwice) state.DoubleNextActive = false;

            ApplyOnce(module);

            if (applyTwice)
                ApplyOnce(module);
        }

        private static void ApplyOnce(ModuleDataItem module)
        {
            switch (module.effectKey)
            {
                case "RepairBrokenSlot":
                    TryRepairBrokenSlot();
                    break;
                case "ConvertFoodToEnergy":
                    TryConvertFoodToEnergy();
                    break;
                case "DoubleNextActiveModule":
                    TryDoubleNextActiveModule(); // 자기 자신은 2번 안 함
                    break;
                case "ReduceOverloadByTech":
                    TryReduceOverloadByTech();
                    break;
                case "IncreaseDataByFood":
                    TryIncreaseDataByFood();
                    break;
                default:
                    Debug.LogWarning($"[ActiveEffect] Unknown key: {module.effectKey}");
                    break;
            }
        }

        private static void TryRepairBrokenSlot()
        {
            var state = GameManager.Instance.PlayerState;
            var board = state.SlotBoard;

            int brokenIndex = board.Rows.FindIndex(active => !active);

            if (brokenIndex == -1)
            {
                Debug.Log("[ActiveEffect] 고장난 슬롯 없음");
                return;
            }

            if (state.Technology >= 2)
            {
                state.Technology -= 2;
                state.Energy -= 1;
                board.UnlockRow(brokenIndex);
                Debug.Log($"[ActiveEffect] 기술 2 소모 → {brokenIndex}번째 슬롯 행 복구 완료");
            }
            else
            {
                Debug.Log("[ActiveEffect] 기술 부족: 복구 실패");
            }
        }

        private static void TryConvertFoodToEnergy()
        {
            var state = GameManager.Instance.PlayerState;

            if (state.Food >= 1)
            {
                state.Food -= 1;
                state.Energy += 2;
                state.Energy -= 1;
                Debug.Log("[ActiveEffect] 식량 1 → 에너지 2 적용됨");
            }
            else
            {
                Debug.Log("[ActiveEffect] 식량 부족: 변환 실패");
            }
        }

        private static void TryDoubleNextActiveModule()
        {
            var state = GameManager.Instance.PlayerState;
            state.DoubleNextActive = true;
            state.Energy -= 1;
            Debug.Log("[ActiveEffect] 다음 액티브 모듈 2회 발동 플래그 설정됨");
        }


        private static void TryReduceOverloadByTech()
        {
            var state = GameManager.Instance.PlayerState;

            if (state.Technology >= 1)
            {
                state.Technology -= 1;
                state.OverloadGauge -= 10f;
                state.Energy -= 1;
                Debug.Log("[ActiveEffect] 기술 1 → 과부하 게이지 10%p 감소 적용됨");
            }
            else
            {
                Debug.Log("[ActiveEffect] 기술 부족: 변환 실패");
            }
        }

        private static void TryIncreaseDataByFood()
        {
            var state = GameManager.Instance.PlayerState;

            if (state.Food >= 1)
            {
                state.Food -= 1;
                state.Data += 2;
                state.Energy -= 1;
                Debug.Log("[ActiveEffect] 식량 1 → 데이터 2 적용됨");
            }
            else
            {
                Debug.Log("[ActiveEffect] 식량 부족: 변환 실패");
            }
        }
    }
}
