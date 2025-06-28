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

        private HashSet<int> brokenModuleIndices = new();

        // shopModules 디버깅용
        [SerializeField] private List<ModuleDataItem> debugShopModuleList = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateDebugShopList();
        }
#endif

        private void UpdateDebugShopList()
        {
            debugShopModuleList.Clear();
            foreach (var kvp in shopModules)
            {
                debugShopModuleList.Add(kvp.Value);
            }
        }
        // shopModules 디버깅용


        /// <summary>
        /// 상점에 진열할 모듈 N개를 무작위로 선택
        /// </summary>
        public void SetupShop()
        {
            GenerateShop();
        }

        public void RerollShop()
        {
            var state = GameManager.Instance.PlayerState;

            if (state.Data < 1)
                return;

            state.Data -= 1;
            GameManager.Instance.playerStateUI.RefreshUI();
            GenerateShop();
        }

        private void GenerateShop()
        {
            shopModules.Clear();

            List<ModuleDataItem> shuffled = new(GameManager.Instance.ModuleDict.Values);
            Shuffle(shuffled);

            for (int i = 0; i < Mathf.Min(4, shuffled.Count); i++)
            {
                shopModules.Add(i, shuffled[i]);
            }

            ModuleShopManager.Instance.RefreshShopUI();
            UpdateDebugShopList();
        }
        public void ClearShop()
        {
            // 1. 현재 상점 프리팹 안전하게 반환
            List<Transform> children = new();
            foreach (Transform child in ModuleShopManager.Instance.shopItemContainer)
                children.Add(child);

            foreach (var child in children)
                ModuleShopPrefabPooler.Instance.Return(child.gameObject);

            // 2. 데이터 초기화
            shopModules.Clear();

            // 3. UI 선택 초기화
            ModuleShopManager.Instance.ClearSelection();

            // 4. 디버그 리스트 업데이트
            UpdateDebugShopList();
        }


        /// <summary>
        /// 상점에 없는 모듈 중 무작위 하나를 추가합니다.
        /// </summary>
        public void AddRandomModuleToShop()
        {
            // 1. 전체 모듈 목록 가져오기
            List<ModuleDataItem> allModules = new(GameManager.Instance.ModuleDict.Values);

            // 2. 현재 상점에 이미 있는 모듈 제거
            foreach (var existing in shopModules.Values)
            {
                allModules.Remove(existing);
            }

            // 3. 남은 게 없다면 리턴
            if (allModules.Count == 0)
            {
                Debug.LogWarning("더 이상 추가할 모듈이 없습니다.");
                return;
            }

            // 4. 기존 Shuffle 메서드로 섞기
            Shuffle(allModules);

            // 5. 첫 번째 모듈 선택 후 추가
            var selectedModule = allModules[0];
            int newIndex = shopModules.Count;
            shopModules[newIndex] = selectedModule;

            ModuleShopManager.Instance.AddNewModuleToShop(newIndex, selectedModule);

            UpdateDebugShopList();
        }




        /// <summary>
        /// 모듈을 구매하고 지정 위치(Row/Column + index)에 장착함
        /// </summary>
        /// 
        public void TryPurchaseAndEquip(int moduleKey, int index)
        {
            if (!shopModules.TryGetValue(moduleKey, out var module))
                return;

            var state = GameManager.Instance.PlayerState;

            if (module.purchaseCost > state.Data)
                return;

            state.Data -= module.purchaseCost;

            // 기존 모듈이 있다면 제거 (덮어쓰기)
            equippedModules.RemoveAll(m => m.index == index);

            equippedModules.Add(new ModuleEquipSlot(module, index));
            shopModules.Remove(moduleKey);

            ModuleShopManager.Instance.ClearSelection();
            ModuleShopManager.Instance.RemoveModuleUI(moduleKey);

            GameManager.Instance.playerStateUI.RefreshUI();
        }

        /// <summary>
        /// 특정 위치(Row/Col + index)에 장착된 모듈 반환
        /// </summary>
        public ModuleDataItem GetEquippedModule(int index)
        {
            foreach (var equip in equippedModules)
            {
                if (equip.index == index)
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

        public void SetModuleBroken(int index)
        {
            brokenModuleIndices.Add(index);
        }

        public void RepairModule(int index)
        {
            var state = GameManager.Instance.PlayerState;
            if (state.Technology <= 0)
                return;

            brokenModuleIndices.Remove(index);
            state.Technology -= 1;

            GameManager.Instance.playerStateUI.RefreshUI();
        }

        public bool IsModuleBroken(int index)
        {
            return brokenModuleIndices.Contains(index);
        }

    }

    /// <summary>
    /// 실제 장착된 모듈 정보: 어떤 모듈이, 어디(Row/Col), 몇 번째 슬롯에 붙었는가
    /// </summary>
    [System.Serializable]
    public class ModuleEquipSlot
    {
        public ModuleDataItem module;
        public int index;

        public ModuleEquipSlot(ModuleDataItem module, int index)
        {
            this.module = module;
            this.index = index;
        }
    }

}
