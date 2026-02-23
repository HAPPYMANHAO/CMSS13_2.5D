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
        InventoryManager.OnInventoryChanged -= Refresh;

        _currentContainer = container;
        _currentInteractor = interactor;
        container.OnLootChanged += Refresh;
        InventoryManager.OnInventoryChanged += Refresh;
        gameObject.SetActive(true);
        Refresh();
    }

    private void OnDisable()
    {
        if (_currentContainer != null)
            _currentContainer.OnLootChanged -= Refresh;
        InventoryManager.OnInventoryChanged -= Refresh;
    }

    private void OnDestroy()
    {
        ItemContainerBase.OnPlayerOpenLootContainer -= HandlePlayerOpenLootContainer;
    }

    private void Refresh()
    {
        BuildList(contentTransform, _currentContainer.GetLoot(), HandleLootItemClicked);
    }

    private void BuildList(Transform parent, List<ItemInstance> items, System.Action<ItemInstance> onClick)
    {
        if (this == null) return;

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
        if (inventoryManager.AddItem(item))
            _currentContainer.TakeItem(item);
    }

    private void HandleTakeAll()
    {
        _currentContainer.TakeAll(inventoryManager);
    }

    private void HandlePlayerOpenLootContainer(ItemContainerBase container, CurrentPartyMemberInfo interactor)
    {
        OpenWith(container, interactor);
    }
}
