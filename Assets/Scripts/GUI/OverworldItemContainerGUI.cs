using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

//这个类显示全部物品，它只能在overworld场景使用
//This class displays all items and can only be used in the overworld scene.
public class OverworldItemContainerGUI : MonoBehaviour
{
    private InventoryManager inventoryManager;
    [SerializeField] private OverworldVisualGUI overworldVisualGUI;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private Button exitButton;
    [SerializeField] private Button backgroundButton; // 收回物品

    public PartyManager pratyManager;


    private void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

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

    private void AddItems(List<ItemInstance> items)
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
        ItemInstance itemInHand = pratyManager.currentPlayerEntity.GetCurrentActiveHandItem();
        if (itemInHand != null)
        {
            inventoryManager.AddItem(itemInHand);
            pratyManager.currentPlayerEntity.SentHoldItemToInventory();
            UpdateCurrentGUI();
        }

    }

    private void HandleItemClicked(ItemInstance item)
    {
        ItemInstance itemInHand = pratyManager.currentPlayerEntity.GetCurrentActiveHandItem();
        if (itemInHand != null) return;

        if (item.itemData is HoldableBase)
        {
            inventoryManager.RemoveItem(item);
            pratyManager.currentPlayerEntity.GetItemFromToInventory(item);
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

        overworldVisualGUI?.UpdateHandVisuals();
    }
}