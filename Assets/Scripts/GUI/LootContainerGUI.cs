using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootContainerGUI : MonoBehaviour
{
    public static LootContainerGUI Instance { get; private set; }//self

    private InventoryManager inventoryManager;
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private Transform contentTransform;

    [SerializeField] private Button takeAllButton;
    [SerializeField] private Button closeButton;

    private ItemContainerBase _currentContainer;
    private CurrentPartyMemberInfo _currentInteractor;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        ItemContainerBase.OnPlayerOpenLootContainer += HandlePlayerOpenLootContainer;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        takeAllButton.onClick.AddListener(HandleTakeAll);     
    }

    public void OpenWith(ItemContainerBase container, CurrentPartyMemberInfo interactor)
    {
        if (_currentContainer != null)
            _currentContainer.OnLootChanged -= Refresh;
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged -= Refresh;

        _currentContainer = container;
        _currentInteractor = interactor;
        container.OnLootChanged += Refresh;
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged += Refresh;
        gameObject.SetActive(true);
        Refresh();
    }

    private void OnDisable()
    {
        // 取消订阅所有事件
        if (_currentContainer != null)
            _currentContainer.OnLootChanged -= Refresh;
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged -= Refresh;
    }
    
    private void OnEnable()
    {
        // 重新订阅事件（如果已有容器）
        if (_currentContainer != null)
            _currentContainer.OnLootChanged += Refresh;
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged += Refresh;
    }

    private void OnDestroy()
    {
        ItemContainerBase.OnPlayerOpenLootContainer -= HandlePlayerOpenLootContainer;
    }

    private void Refresh()
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null || _currentContainer == null) return;
        
        BuildList(contentTransform, _currentContainer.GetLoot(), HandleLootItemClicked);
    }

    private void BuildList(Transform parent, List<ItemInstance> items, System.Action<ItemInstance> onClick)
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null || contentTransform == null) return;

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            GameObject go = Instantiate(itemPrefab, contentTransform);
            ItemBaseGUI itemGUI = go.GetComponent<ItemBaseGUI>();


            if (itemGUI != null)
            {
                itemGUI.Setup(item, HandleLootItemClicked);
            }
        }
    }

    private void HandleLootItemClicked(ItemInstance item)
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null) return;
        
        if (inventoryManager != null && _currentContainer != null && item != null)
        {
            if (inventoryManager.AddItem(item))
                _currentContainer.TakeItem(item);
        }
    }

    private void HandleTakeAll()
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null) return;
        
        if (inventoryManager != null && _currentContainer != null)
        {
            _currentContainer.TakeAll(inventoryManager);
        }
    }

    private void HandlePlayerOpenLootContainer(ItemContainerBase container, CurrentPartyMemberInfo interactor)
    {
        OpenWith(container, interactor);
    }
}
