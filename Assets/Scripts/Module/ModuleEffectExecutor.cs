using UnityEngine;

namespace EmergencyRoulette
{
    public static class ModuleEffectExecutor
    {
        public static void ApplyAllPassiveModules()
        {
            var player = GameManager.Instance.PlayerState;

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
                        GameManager.Instance.PlayerState.SetGainDataOnDanger();
                        Debug.Log("[PassiveEffect] 위험 2개 이상 시 데이터 +2 (미구현)");
                        break;

                    default:
                        Debug.LogWarning($"[PassiveEffect] Unknown key: {module.effectKey}");
                        break;
                }
            }
        }


        public static void ApplyActiveModuleAt(int index)
        {
            var module = ModuleManager.Instance.GetEquippedModule(index);
            if (module == null || module.useType != ModuleUseType.Active)
                return;

            // 에너지 부족 체크 등은 여기서 수행
            switch (module.effectKey)
            {
                case "RepairBrokenSlot":
                    Debug.Log("슬롯 복구 (미구현)");
                    break;
                case "ConvertFoodToEnergy":
                    Debug.Log("식량 → 에너지 (미구현)");
                    break;
                case "DoubleNextActiveModule":
                    Debug.Log("다음 액티브 모듈 2번 (미구현)");
                    break;
                case "ReduceOverloadByTech":
                    Debug.Log("과부하 감소 (미구현)");
                    break;
                default:
                    Debug.LogWarning($"[ActiveEffect] Unknown key: {module.effectKey}");
                    break;
            }
        }
    }
}
