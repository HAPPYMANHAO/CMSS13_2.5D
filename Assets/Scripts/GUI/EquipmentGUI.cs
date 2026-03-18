using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots;
    [SerializeField] private List<SlotUI> storageSlots;
    private InventoryManager inventoryManager;
    private PartyManager partyManager;
    private BattleEntityManager battleEntityManager;
    [SerializeField] private Button menuButton;

    private bool isEquipmentMenuOpen = false;
    private void Start()
    {
        inventoryManager = InventoryManager.instance; 
        BattleEntityManager entityManager = FindFirstObjectByType<BattleEntityManager>();
        if (entityManager != null)
        {
            battleEntityManager = entityManager;
        }
        else
        {
            partyManager = PartyManager.instance;
        }
        

        CurrentPartyMemberInfo.OnEquipmentChanged += Refresh;

        foreach (var slot in slots)
        {
            var capturedSlot = slot; // 捕获变量
            slot.button.onClick.AddListener(() => HandleSlotClicked(capturedSlot.slotType));
        }
        foreach (var slot in storageSlots)
        {
            var capturedSlot = slot; // 捕获变量
            slot.button.onClick.AddListener(() => HandleSlotClicked(capturedSlot.slotType));
        }

        menuButton.onClick.AddListener(HandlemenuButtonClicked);

        if(GameStateManager.instance.currentGameState == GameState.Battle)
        {
            BattleEntityManager.OnPartyEntitiesSpawned += Refresh;
        }
        else
        {
            Refresh();
        }
    }

    private void OnDisable()
    {
        BattleEntityManager.OnPartyEntitiesSpawned -= Refresh;
    }

    private IEquipmentOwner GetCurrentPlayer()
    {
        if(battleEntityManager != null)
        {
            return battleEntityManager.currentPlayerEntity as IEquipmentOwner;
        }
        else
        {
            return partyManager.currentPlayerEntity as IEquipmentOwner;
        }
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
        var player = GetCurrentPlayer();
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
        foreach(var slot in storageSlots)
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

    // 点击槽位：如果有装备就卸下到背包
    //TODO
    private void HandleSlotClicked(EquipmentSlotType slotType)
    {
        var player = GetCurrentPlayer();
        var currentEquipped = player.GetEquipped(slotType);

        if (currentEquipped != null)
        {
            // 卸下 → 放回背包
            player.UnequipSlot(slotType);
            inventoryManager.AddItem(currentEquipped);
        }

        Refresh();
        PartyManager.PartyUpdated();
    }
}
