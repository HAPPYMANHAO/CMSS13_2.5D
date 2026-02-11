using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public void UpdateItemContainerGUI(List<ItemBase> items)
    {
        AddItems(items);
    }

    private void AddItems(List<ItemBase> items)
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
        ItemBase itemInHand = battleEntityManager.currentPlayerEntity.GetCurrentActiveHandItem();
        if (itemInHand != null)
        {
            inventoryManager.AddItemToInventory(itemInHand);
            battleEntityManager.currentPlayerEntity.SentHoldItemToInventory();
        }

        UpdateItemContainerGUI(inventoryManager.GetCurrentInventoryItems());
        battleVisualGUI.UpdateHandVisuals();
    }

    private void HandleItemClicked(ItemBase item)
    {
        if (item is HoldableBase holdable)
        {
            // 执行装备逻辑TODO
        }
    }
}
