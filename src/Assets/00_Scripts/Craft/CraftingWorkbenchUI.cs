using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.Crafting.UI;
using HBDinosaur_ER_Project.InventoryRewrite;
using HBDinosaur_ER_Project.ItemData;
using HBDinosaur_ER_Project.StorageSystem;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftingWorkbenchUI : MonoBehaviour
    {
        [SerializeField] private CraftRecipeDatabase recipeDatabase;
        [SerializeField] private CraftTreeRenderer treeRenderer;
        [SerializeField] private CraftSearchTable searchTable;
        [SerializeField] private CraftSelectView selectView;
        [SerializeField] private int defaultItemId;

        private CraftingService craftingService;
        private CraftTreeNode currentRootNode;

        private CraftTreeBuilder treeBuilder;
        private int currentRootItemId;

        // 중요: 최초 1회 sub-UI들이 초기화되었는지 확인하는 플래그
        private bool isSubUIInitialized = false;

        private void Awake()
        {
            treeRenderer = GetComponent<CraftTreeRenderer>();
            treeBuilder = new CraftTreeBuilder(recipeDatabase);
            // 캐싱용으로 미리 세팅
            currentRootItemId = defaultItemId;
        }
        private void OnEnable()
        {
            // 1. 패널이 열릴 때마다 싱글톤(StorageManager)의 최신 인스턴스를 가져옵니다.
            NewStorageManager storageManager = NewStorageManager.Instance;

            if (storageManager == null)
            {
                Debug.LogError("[CraftingWorkbenchUI] StorageManager.Instance를 찾을 수 없습니다! 타이밍을 확인하세요.", this);
                return;
            }

            Debug.Log($"[CraftingWorkbenchUI] OnEnable - StorageManager={storageManager}", this);
            Debug.Log($"[CraftingWorkbenchUI] OnEnable - PlayerInventory={storageManager.PlayerInventory}", this);
            Debug.Log($"[CraftingWorkbenchUI] OnEnable - Storage={storageManager.Storage}", this);

            // 2. 열릴 때마다 최신 인벤토리/창고 상태를 반영한 어댑터와 서비스 생성
            CraftingStorageAdapter storageAdapter = new CraftingStorageAdapter(
                storageManager.PlayerInventory,
                storageManager.Storage);

            craftingService = new CraftingService(storageAdapter);

            // 3. 서브 UI 요소들(SelectView, SearchTable)은 딱 한 번만 콜백을 연결해 줍니다.
            if (!isSubUIInitialized)
            {
                if (selectView != null)
                    selectView.Initialize(OnClick_Craft);

                if (searchTable != null)
                    searchTable.Initialize(ShowTree, recipeDatabase);

                isSubUIInitialized = true;
            }

            // 4. 패널이 열렸을 때, 이전에 보던 아이템 ID 혹은 기본 아이템 ID로 트리 새로고침
            if (currentRootItemId > 0)
            {
                ShowTree(currentRootItemId);
            }
        }

        private void Start()
        {
            NewStorageManager storageManager = NewStorageManager.Instance;

            Debug.Log($"[CraftingWorkbenchUI] StorageManager={NewStorageManager.Instance}", this);
            Debug.Log($"[CraftingWorkbenchUI] PlayerInventory={NewStorageManager.Instance?.PlayerInventory}", this);
            Debug.Log($"[CraftingWorkbenchUI] Storage={NewStorageManager.Instance?.Storage}", this);

            CraftingStorageAdapter storageAdapter = new CraftingStorageAdapter(
                NewStorageManager.Instance.PlayerInventory,
                NewStorageManager.Instance.Storage);

            craftingService = new CraftingService(storageAdapter);

            if (selectView != null)
                selectView.Initialize(OnClick_Craft);

            if (searchTable != null)
                searchTable.Initialize(ShowTree, recipeDatabase);

            if(defaultItemId >0)
                ShowTree(defaultItemId);
        }

        public void ShowTree(int rootItemId)
        {
            currentRootItemId = rootItemId;

            currentRootNode = treeBuilder.BuildTree(rootItemId);
            treeRenderer.Render(currentRootNode, craftingService, OnNodeClicked);
            if (selectView != null)
            {
                ItemDataStruct? data = ItemDatabase.Instance.GetItemByID(rootItemId);
                selectView.SetData(currentRootNode, data);
            }
        }

        private void OnNodeClicked(CraftTreeNode clickedNode)
        {
            if (clickedNode == null)
                return;

            ShowTree(clickedNode.ItemId);
        }

        public void ShowCurrentTree()
        {
            ShowTree(currentRootItemId);
        }
        public void OnClick_Craft()
        {
            if (craftingService == null)
            {
                Debug.LogError("[CraftingWorkbenchUI] CraftingService is null.", this);
                return;
            }

            if (currentRootNode == null)
            {
                Debug.LogWarning("[CraftingWorkbenchUI] Current root node is null.", this);
                return;
            }

            Debug.Log($"[CraftingWorkbenchUI] Try craft itemId={currentRootNode.ItemId}, amount={currentRootNode.NeedAmount}", this);

            var requiredItems = craftingService.GetRequiredItems(currentRootNode);

            Debug.Log("[CraftingWorkbenchUI] Required materials:", this);

            foreach (var pair in requiredItems)
            {
                int itemId = pair.Key;
                int requiredAmount = pair.Value;
                int ownedAmount = craftingService.GetTotalItemCount(itemId);

                Debug.Log($"- ItemId={itemId}, Required={requiredAmount}, Owned={ownedAmount}", this);
            }

            var missingItems = craftingService.GetMissingItems(currentRootNode);

            if (missingItems.Count > 0)
            {
                Debug.LogWarning("[CraftingWorkbenchUI] Craft failed. Missing materials:", this);

                foreach (var pair in missingItems)
                {
                    Debug.LogWarning($"- ItemId={pair.Key}, Missing={pair.Value}", this);
                }

                return;
            }

            bool success = craftingService.TryCraft(currentRootNode);

            if (!success)
            {
                Debug.LogWarning($"[CraftingWorkbenchUI] Craft failed during consume/add. itemId={currentRootNode.ItemId}", this);
                return;
            }

            Debug.Log($"[CraftingWorkbenchUI] Craft success. itemId={currentRootNode.ItemId}, amount={currentRootNode.NeedAmount}", this);

            if (UIItemMoveManager.Instance != null)
            {
                // UIItemMoveManager에서 인벤토리 어댑터를 가져옵니다.
                var invContainer = UIItemMoveManager.Instance.GetContainer(ItemContainerType.Inventory);
                if (invContainer != null)
                {
                    invContainer.RefreshUI(); // 화면의 인벤토리 슬롯들을 다시 그립니다.
                    Debug.Log("[CraftingWorkbenchUI] Inventory UI Refreshed via MoveManager.");
                }

                // 혹시 창고(Storage) 재료도 쓰였다면 창고 UI도 함께 새로고침해 줍니다.
                var storageContainer = UIItemMoveManager.Instance.GetContainer(ItemContainerType.Storage);
                if (storageContainer != null && UIItemMoveManager.Instance.IsUIOpen(ItemContainerType.Storage))
                {
                    storageContainer.RefreshUI();
                }
            }

            ShowCurrentTree();
        }
    }
}
