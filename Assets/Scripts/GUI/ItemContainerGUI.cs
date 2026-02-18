using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ItemContainerGUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private BattleVisualGUI battleVisualGUI;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button backgroundButton; // 收回物品

    public BattleEntityManager battleEntityManager;

    private void Start()
    {
        exitButton.onClick.AddListener(HandleExitButtonPressed);
        backgroundButton.onClick.AddListener(HandBackgroundButtonPressed);

    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += HandleInventoryChanged;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= HandleInventoryChanged;
    }

    public void UpdateItemContainerGUI(List<ItemInstance> items)
    {
        AddItems(items);
    }

    private void AddItems(List<ItemInstance>items)
    {
        if (this == null || scrollRect == null) return;

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            GameObject go = Instantiate(ItemPrefab, contentTransform);
            ItemBaseGUI itemGUI = go.GetComponent<ItemBaseGUI>();


            if (itemGUI != null)
            {
                itemGUI.Setup(item, HandleItemClicked);
            }
        }
    }

    //------------------Event-------------------//

    private void HandleExitButtonPressed()
    {
        gameObject.SetActive(false);
    }
    private void HandBackgroundButtonPressed()
    {
        ItemInstance itemInHand = battleEntityManager.currentPlayerEntity.GetCurrentActiveHandItem();
        if (itemInHand != null)
        {
            inventoryManager.AddItem(itemInHand);
            battleEntityManager.currentPlayerEntity.SentHoldItemToInventory();
            UpdateCurrentGUI();
        }

    }

    private void HandleItemClicked(ItemInstance item)
    {
        ItemInstance itemInHand = battleEntityManager.currentPlayerEntity.GetCurrentActiveHandItem();
        if (itemInHand != null) return;

        if (item.itemData is HoldableBase)
        {
            inventoryManager.RemoveItem(item);
            battleEntityManager.currentPlayerEntity.GetItemFromToInventory(item);
            UpdateCurrentGUI();
        }
    }

    private void HandleInventoryChanged()
    {
        if (gameObject.activeSelf)
            UpdateCurrentGUI();
    }

    private void UpdateCurrentGUI()
    {
        UpdateItemContainerGUI(inventoryManager.GetAllItems());

        battleVisualGUI.UpdateHandVisuals();
    }
}
