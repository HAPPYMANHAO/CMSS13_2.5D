using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots;
    [SerializeField] private List<SlotUI> storageSlots;
    private InventoryManager inventoryManager;
    [SerializeField] private Button menuButton;
    [SerializeField] private BaseVisualGUI visualGUI;

    private bool isEquipmentMenuOpen = false;
    private bool _isSubscribed = false;

    private void Start()
    {
        inventoryManager = InventoryManager.instance;       

        // 订阅事件
        SubscribeEvents();

        foreach (var slot in slots)
        {
            if (slot == null || slot.button == null) continue;
            var capturedSlot = slot; // 捕获变量
            slot.button.onClick.AddListener(() => HandleSlotClicked(capturedSlot.slotType));
        }
        foreach (var slot in storageSlots)
        {
            if (slot == null || slot.button == null) continue;
            var capturedSlot = slot; // 捕获变量
            slot.button.onClick.AddListener(() => HandleSlotClicked(capturedSlot.slotType));
        }

        if (menuButton != null)
            menuButton.onClick.AddListener(HandlemenuButtonClicked);

        if(GameStateManager.instance != null && GameStateManager.instance.currentGameState == GameState.Battle)
        {
            BattleEntityManager.OnPartyEntitiesSpawned += Refresh;
        }
        else
        {
            Refresh();
        }
    }

    private void SubscribeEvents()
    {
        if (_isSubscribed) return;
        
        CurrentPartyMemberInfo.OnEquipmentChanged += Refresh;
        _isSubscribed = true;
    }

    private void UnsubscribeEvents()
    {
        if (!_isSubscribed) return;
        
        CurrentPartyMemberInfo.OnEquipmentChanged -= Refresh;
        BattleEntityManager.OnPartyEntitiesSpawned -= Refresh;
        _isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private IEquipmentOwner GetCurrentPlayer()
    {
        if (visualGUI == null) return null;
        return visualGUI.GetCurrentPlayer() as IEquipmentOwner;
    }

    public void HandlemenuButtonClicked()
    {
        if (this == null || gameObject == null) return;

        isEquipmentMenuOpen = !isEquipmentMenuOpen;

        foreach (var slot in slots)
        {
            if (slot != null && slot.gameObject != null)
                slot.gameObject.SetActive(isEquipmentMenuOpen);
        }

        Refresh();
    }

    private void Refresh()
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null) return;

        var player = GetCurrentPlayer();
        if (player == null) return;

        // 检查并清理已销毁的槽位
        slots.RemoveAll(slot => slot == null || slot.icon == null);
        storageSlots.RemoveAll(slot => slot == null || slot.icon == null);

        foreach (var slot in slots)
        {
            if (slot == null || slot.icon == null) continue;

            var equipped = player.GetEquipped(slot.slotType);
            bool hasItem = equipped != null;

            slot.icon.sprite = hasItem ? equipped.itemData.icon : null;
            if (hasItem)
            {
                slot.EnableItemSprite();
            }
            else
            {
                slot.DisableItemSprite();
            }
            slot.icon.gameObject.SetActive(hasItem);
        }

        foreach (var slot in storageSlots)
        {
            if (slot == null || slot.icon == null) continue;

            var equipped = player.GetEquipped(slot.slotType);
            bool hasItem = equipped != null;

            slot.icon.sprite = hasItem ? equipped.itemData.icon : null;
            if (hasItem)
            {
                slot.EnableItemSprite();
            }
            else
            {
                slot.DisableItemSprite();
            }
            slot.icon.gameObject.SetActive(hasItem);
        }
    }

    // 点击槽位：如果有装备就卸下到背包，如果是存储就打开它
    //TODO
    private void HandleSlotClicked(EquipmentSlotType slotType)
    {
        // 检查对象是否有效
        if (this == null || gameObject == null) return;

        var player = GetCurrentPlayer();
        if (player == null) return;

        var currentEquipped = player.GetEquipped(slotType);
        if (currentEquipped != null && currentEquipped is StorageItemInstance)
        {
            var storageItem = (StorageItemInstance)currentEquipped;
            if (visualGUI != null)
                visualGUI.ContainerUpdate(storageItem);
        }
        else 
        {
            // 卸下 → 放回背包
            var unequippedItem = player.UnequipSlot(slotType);
            if (unequippedItem != null && inventoryManager != null)
            {
                inventoryManager.AddItem(unequippedItem);
            }
        }
        
        Refresh();
        PartyManager.PartyUpdated();
    }
}
