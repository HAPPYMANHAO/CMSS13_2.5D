using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ItemContainerGUI : MonoBehaviour
{
    private IStorage currentContainer;
    private GameStateManager gameStateManager;
    [SerializeField] private BaseVisualGUI visualGUI;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Image BackgroundTipColor; 

    [SerializeField] private Button exitButton;
    [SerializeField] private Button backgroundButton; // 收回物品

    public bool isItemStorage = false;

    private void Awake()
    {
        // 延迟初始化，避免执行顺序问题
        currentContainer = null;
        gameStateManager = null;
    }
    
    private void Start()
    {
        // 在 Start 中安全地获取实例
        if (currentContainer == null)
            currentContainer = InventoryManager.instance;
        if (gameStateManager == null)
            gameStateManager = GameStateManager.instance;

        exitButton.onClick.AddListener(HandleExitButtonPressed);
        backgroundButton.onClick.AddListener(HandBackgroundButtonPressed);
    }

    private void OnEnable()
    {
        // 订阅事件（只在 OnEnable 中订阅，避免与 Start 重复）
        if (currentContainer != null)
            currentContainer.OnInventoryChanged += HandleInventoryChanged;
    }
    
    private void OnDisable()
    {
        // 取消订阅事件
        if (currentContainer != null)
            currentContainer.OnInventoryChanged -= HandleInventoryChanged;
    }
    
    private void OnDestroy()
    {
        // 安全网：确保取消订阅（即使 currentContainer 可能为 null）
        if (currentContainer != null)
            currentContainer.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void UpdateItemContainerGUI(List<ItemInstance> items)
    {
        AddItems(items);
        this.gameObject.SetActive(true);
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
        ItemInstance itemInHand = visualGUI.GetCurrentPlayer().GetCurrentActiveHandItem();
        if (itemInHand != null)
        {
            bool allowPutIn = currentContainer.AddItem(itemInHand);
            if (allowPutIn)
            {
                visualGUI.GetCurrentPlayer().SentHoldItemToInventory();
                UpdateCurrentGUI();
            }
        }
    }

    private void HandleItemClicked(ItemInstance item)
    {
        var gun = visualGUI.GetCurrentPlayer().GetCurrentActiveHandItem() as GunInstance;

        // 手里有枪，点击的是匹配弹药 → 装弹
        if (gun != null && item is StackableItemInstance ammoStack
            && ammoStack.itemData is AmmoBase ammo
            && ammo.ammoType == gun.GunData.acceptedAmmoType)
        {
            int loaded = gun.Reload(ammoStack);
            if (ammoStack.IsEmpty)
                currentContainer.RemoveItem(ammoStack);
            UpdateCurrentGUI();
            // Log
            return;
        }

        ItemInstance itemInHand = visualGUI.GetCurrentPlayer().GetCurrentActiveHandItem();
        if (itemInHand != null) return;

        if (item.itemData is HoldableBase)
        {
            currentContainer.RemoveItem(item);
            visualGUI.GetCurrentPlayer().GetItemFromToInventory(item);
            // 播放物品进入手的动画
            visualGUI.UpdateHandVisuals(true);
            UpdateCurrentGUI();
        }
        else if ((item.itemData is ArmorItemBase || item.itemData is StorageItemBase) && gameStateManager.currentGameState == GameState.Overworld)
        {
            bool removed = currentContainer.RemoveItem(item);
            if (removed)
            {
                bool equipped = PartyManager.instance.currentPlayerEntity.TryEquip(item);
                if (equipped)
                {
                    UpdateCurrentGUI();
                }
                else
                {
                    // 装备失败，将物品返回到容器
                    currentContainer.AddItem(item);
                }
            }
        }
    }

    private void HandleInventoryChanged()
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null) return;
        
        if (gameObject.activeSelf)
            UpdateCurrentGUI();
    }

    /// <summary>
    /// 获得物品并且更新GUI，
    /// </summary>
    public void UpdateCurrentGUI()
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null || currentContainer == null) return;
        
        if (isItemStorage)
        {
            UpdateBackgroundButtonColor();
        }
        UpdateItemContainerGUI(currentContainer.GetAllItems());
        this.gameObject.SetActive(true);
        if (visualGUI != null)
            visualGUI.UpdateHandVisuals();
    }
    /// <summary>
    /// 获得存储item物品并且更新GUI
    /// </summary>
    public void UpdateCurrentGUI(IStorage storage)
    {
        if (storage == null) return;
        
        // 先取消旧容器的事件订阅
        if (currentContainer != null)
            currentContainer.OnInventoryChanged -= HandleInventoryChanged;
        
        // 设置新容器
        currentContainer = storage;
        isItemStorage = storage is StorageItemInstance;
        
        // 订阅新容器的事件（如果游戏对象已启用）
        if (gameObject.activeInHierarchy && currentContainer != null)
            currentContainer.OnInventoryChanged += HandleInventoryChanged;
        
        // 更新背景颜色
        if (isItemStorage)
            UpdateBackgroundButtonColor();
        else
            BackgroundTipColor.color = Color.black;

        UpdateItemContainerGUI(currentContainer.GetAllItems());
        visualGUI.UpdateHandVisuals();
    }

    private void UpdateBackgroundButtonColor()
    {
        if(currentContainer is StorageItemInstance)
        {
            var storageItem = currentContainer as StorageItemInstance;
            if (storageItem.currentOccupiedVolume / storageItem.capacity >= ContainerThreshold.CONTAINER_FULL_100)
            {
                BackgroundTipColor.color = Color.red;
            }
            else if (storageItem.currentOccupiedVolume / storageItem.capacity >= ContainerThreshold.CONTAINER_ALMOST_FULL_80)
            {
                BackgroundTipColor.color = Color.yellow;
            }
            else
            {
                BackgroundTipColor.color = Color.black;
            }
        }
    }
}
