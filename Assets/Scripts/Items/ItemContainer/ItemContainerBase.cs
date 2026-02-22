using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerBase : MonoBehaviour, IInteractable
{
    [SerializeField] private LootTable lootTable;   // 掉落表

    private List<ItemInstance> _lootItems;
    private bool _hasGenerated = false;

    // 事件
    public event Action OnLootChanged;
    public static event Action<ItemContainerBase, CurrentPartyMemberInfo> OnPlayerOpenLootContainer;

    // ── IInteractable ──────────────────────────
    public bool CanInteract(ItemInstance handItem) => true;

    public void Interact(ItemInstance handItem, CurrentPartyMemberInfo interactor)
    {
        if (!_hasGenerated)
        {
            _lootItems = lootTable != null
                ? lootTable.GenerateLoot()
                : new List<ItemInstance>();
            _hasGenerated = true;
        }
        // 通知 GUI 打开

        OnPlayerOpenLootContainer?.Invoke(this, interactor);
    }

    // ── 数据操作 ───────────────────────────────
    public List<ItemInstance> GetLoot()
    {
        if (_lootItems == null)
            return new List<ItemInstance>();

        return new List<ItemInstance>(_lootItems);
    }

    public bool IsEmpty => _lootItems == null || _lootItems.Count == 0;

    public bool TakeItem(ItemInstance item)
    {
        bool removed = _lootItems.Remove(item);
        if (removed) OnLootChanged?.Invoke();
        return removed;
    }

    public void TakeAll(InventoryManager inventory)
    {
        foreach (var item in new List<ItemInstance>(_lootItems))
        {
            inventory.AddItem(item);
            _lootItems.Remove(item);
        }
        OnLootChanged?.Invoke();
    }
}
