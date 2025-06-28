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
                        Debug.Log($"[PassiveEffect] ������ +1");
                        break;

                    case "AddFoodPerTurn":
                        player.Food += 1;
                        Debug.Log($"[PassiveEffect] �ķ� +1");
                        break;

                    case "AddTechPerTurn":
                        player.Technology += 1;
                        Debug.Log($"[PassiveEffect] ��� +1");
                        break;

                    case "AddEnergyPerTurn":
                        player.Energy += 1;
                        Debug.Log($"[PassiveEffect] ������ +1");
                        break;

                    case "IgnoreWarningWithEnergy":
                        // playerState �� ����
                        Debug.Log("[PassiveEffect] ��� ��ȿȭ");
                        break;

                    case "GainDataIfMultipleDanger":
                        GameManager.Instance.PlayerState.SetGainDataOnDanger();
                        Debug.Log("[PassiveEffect] ���� 2�� �̻� �� ������ +2 (�̱���)");
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

            // ������ ���� üũ ���� ���⼭ ����
            switch (module.effectKey)
            {
                case "RepairBrokenSlot":
                    Debug.Log("���� ���� (�̱���)");
                    break;
                case "ConvertFoodToEnergy":
                    Debug.Log("�ķ� �� ������ (�̱���)");
                    break;
                case "DoubleNextActiveModule":
                    Debug.Log("���� ��Ƽ�� ��� 2�� (�̱���)");
                    break;
                case "ReduceOverloadByTech":
                    Debug.Log("������ ���� (�̱���)");
                    break;
                default:
                    Debug.LogWarning($"[ActiveEffect] Unknown key: {module.effectKey}");
                    break;
            }
        }
    }
}
