using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace EmergencyRoulette
{
    public static class ModuleEffectExecutor
    {
        public static void ApplyAllPassiveModules(PlayerState player)
        {
            foreach (var equip in ModuleManager.Instance.equippedModules)
            {
                if (ModuleManager.Instance.IsModuleBroken(equip.index))
                {
                    Debug.Log($"[PassiveEffect] Row {equip.index} ���� �� {equip.module.moduleName} ���� �� ��");
                    continue;
                }

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
                        // playerState �� ����
                        Debug.Log("[PassiveEffect] ���� 2�� �̻� �� ������ +2");
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

            if (ModuleManager.Instance.IsModuleBroken(index))
            {
                Debug.Log($"[ActiveEffect] ���峭 ����� ����� �� �����ϴ� (index: {index})");
                return;
            }

            if (ModuleManager.Instance.UsedActiveModulesThisTurn.Contains(module))
            {
                Debug.Log($"[ActiveEffect] �̹� �Ͽ� �̹� ����� ����Դϴ�: {module.moduleName}");
                return;
            }

            bool applyTwice = state.DoubleNextActive && module.effectKey != "DoubleNextActive";
            if (applyTwice) state.DoubleNextActive = false;

            ApplyOnce(module);

            if (applyTwice)
                ApplyOnce(module);

            // ��� ��Ͽ� �߰�
            GameManager.Instance.playerStateUI.RefreshUI();
            ModuleManager.Instance.UsedActiveModulesThisTurn.Add(module);
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
                    TryDoubleNextActiveModule(); // �ڱ� �ڽ��� 2�� �� ��
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
                Debug.Log("[ActiveEffect] ���峭 ���� ����");
                return;
            }

            if (state.Technology >= 2)
            {
                state.Technology -= 2;
                state.Energy -= 1;
                GameManager.SlotManager.UnlockRow(brokenIndex);
                Debug.Log($"[ActiveEffect] ��� 2 �Ҹ� �� {brokenIndex}��° ���� �� ���� �Ϸ�");
            }
            else
            {
                Debug.Log("[ActiveEffect] ��� ����: ���� ����");
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
                Debug.Log("[ActiveEffect] �ķ� 1 �� ������ 2 �����");
            }
            else
            {
                Debug.Log("[ActiveEffect] �ķ� ����: ��ȯ ����");
            }
        }

        private static void TryDoubleNextActiveModule()
        {
            var state = GameManager.Instance.PlayerState;
            state.DoubleNextActive = true;
            state.Energy -= 1;
            Debug.Log("[ActiveEffect] ���� ��Ƽ�� ��� 2ȸ �ߵ� �÷��� ������");
        }


        private static void TryReduceOverloadByTech()
        {
            var state = GameManager.Instance.PlayerState;

            if (state.Technology >= 1)
            {
                state.Technology -= 1;
                state.OverloadGauge -= 10f;
                state.Energy -= 1;
                Debug.Log("[ActiveEffect] ��� 1 �� ������ ������ 10%p ���� �����");
            }
            else
            {
                Debug.Log("[ActiveEffect] ��� ����: ��ȯ ����");
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
                Debug.Log("[ActiveEffect] �ķ� 1 �� ������ 2 �����");
            }
            else
            {
                Debug.Log("[ActiveEffect] �ķ� ����: ��ȯ ����");
            }
        }
    }
}
