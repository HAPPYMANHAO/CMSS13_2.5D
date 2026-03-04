using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots;
    private InventoryManager inventoryManager;
    private PartyManager partyManager;
    [SerializeField] private Button menuButton;

    private bool isEquipmentMenuOpen = false;

    private void Start()
    {
        inventoryManager = InventoryManager.instance;
        partyManager = PartyManager.instance;

        foreach (var slot in slots)
        {
            var capturedSlot = slot; // 捕获变量
            slot.button.onClick.AddListener(() => HandleSlotClicked(capturedSlot.slotType));
        }

        menuButton.onClick.AddListener(HandlemenuButtonClicked);
        Refresh();
    }

    public void HandlemenuButtonClicked()
    {
        isEquipmentMenuOpen = !isEquipmentMenuOpen;

        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(isEquipmentMenuOpen);
        }

        Refresh();
    }

    private void Refresh()
    {
        var player = partyManager.currentPlayerEntity;
        foreach (var slot in slots)
        {
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

    // 点击槽位：如果有装备就卸下到背包；如果空着就从背包中拾取对应装备，不完善，仅供测试
    //TODO
    private void HandleSlotClicked(EquipmentSlotType slotType)
    {
        var player = partyManager.currentPlayerEntity;
        var currentEquipped = player.GetEquipped(slotType);

        if (currentEquipped != null)
        {
            // 卸下 → 放回背包
            player.UnequipSlot(slotType);
            inventoryManager.AddItem(currentEquipped);
        }
        else
        {
            // 从背包找第一个匹配槽位的装备
            var matchingItem = inventoryManager.GetAllItems()
                .FirstOrDefault(item =>
                    (item.itemData is ArmorItemBase a && a.slotType == slotType) ||
                    (item.itemData is StorageItemBase s && s.slotType == slotType));

            if (matchingItem != null)
            {
                inventoryManager.RemoveItem(matchingItem);
                player.TryEquip(matchingItem);
            }
        }

        Refresh();
        PartyManager.PartyUpdated();
    }
}
