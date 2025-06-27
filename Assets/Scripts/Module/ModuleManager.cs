using System.Collections.Generic;
using UnityEngine;

namespace EmergencyRoulette
{
    public class ModuleManager : MonoBehaviour
    {
        public static ModuleManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        // 이번 상점에 진열된 모듈들
        public Dictionary<int, ModuleDataItem> shopModules = new();

        // 슬롯머신에 장착된 모듈들
        public List<ModuleEquipSlot> equippedModules = new();

        [SerializeField] public ModuleShopManager shopManager;



        /// <summary>
        /// 상점에 진열할 모듈 N개를 무작위로 선택
        /// </summary>
        public void SetupShop(int count)
        {
            shopModules.Clear();

            // 딕셔너리 값들을 리스트로 변환 (모듈 전체 목록)
            List<ModuleDataItem> shuffled = new(GameManager.Instance.ModuleDict.Values);
            Shuffle(shuffled);

            // N개만 상점에 진열
            for (int i = 0; i < Mathf.Min(count, shuffled.Count); i++)
            {
                shopModules.Add(i, shuffled[i]);
            }

            shopManager.RefreshShopUI();
        }



        /// <summary>
        /// 모듈을 구매하고 지정 위치(Row/Column + index)에 장착함
        /// </summary>
        public bool TryPurchaseAndEquip(int moduleKey, EquipAxis axis, int index)
        {
            // 상점에 해당 키가 없으면 실패
            if (!shopModules.TryGetValue(moduleKey, out var module))
                return false;

            // 중복 장착 방지
            foreach (var equip in equippedModules)
            {
                if (equip.axis == axis && equip.index == index)
                    return false;
            }

            equippedModules.Add(new ModuleEquipSlot(module, axis, index));
            shopModules.Remove(moduleKey);

            return true;
        }


        /// <summary>
        /// 특정 위치에 장착된 모듈 해제
        /// </summary>
        public void UnequipModule(EquipAxis axis, int index)
        {
            equippedModules.RemoveAll(m => m.axis == axis && m.index == index);
        }

        /// <summary>
        /// 특정 위치(Row/Col + index)에 장착된 모듈 반환
        /// </summary>
        public ModuleDataItem GetEquippedModule(EquipAxis axis, int index)
        {
            foreach (var equip in equippedModules)
            {
                if (equip.axis == axis && equip.index == index)
                    return equip.module;
            }
            return null;
        }

        /// <summary>
        /// 리스트 무작위 셔플
        /// </summary>
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }
    }

    /// <summary>
    /// 실제 장착된 모듈 정보: 어떤 모듈이, 어디(Row/Col), 몇 번째 슬롯에 붙었는가
    /// </summary>
    [System.Serializable]
    public class ModuleEquipSlot
    {
        public ModuleDataItem module;
        public EquipAxis axis;
        public int index;

        public ModuleEquipSlot(ModuleDataItem module, EquipAxis axis, int index)
        {
            this.module = module;
            this.axis = axis;
            this.index = index;
        }
    }

    /// <summary>
    /// 장착 위치 축: Row 또는 Column
    /// </summary>
    public enum EquipAxis
    {
        Row,
        Column
    }
}
