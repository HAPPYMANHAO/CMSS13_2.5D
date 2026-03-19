using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/StorageItem")]
public class StorageItemBase : ItemBase
{
    public bool isHolster = false;
    [Header("Storage")]
    public EquipmentSlotType slotType; // 属于哪个槽（Back/Belt/Pocket）
    public int storageCapacity;        // 提供几个存储格
    public StorageItemType storageItemType = StorageItemType.Volume;
 
    public override ItemInstance CreateInstance()
        => new StorageItemInstance(this);

    public enum StorageItemType
    {
        Slot,
        Volume,
    }
}

// StorageItemInstance.cs — 运行时实例，持有实际存储的物品
public class StorageItemInstance : ItemInstance, IStorage
{
    private StorageItemBase StorageData => itemData as StorageItemBase;

    public int capacity => StorageData.storageCapacity;
    public bool IsFull => currentOccupiedVolume >= capacity;
    public int currentOccupiedVolume = 0;

    private readonly List<ItemInstance> _slots = new List<ItemInstance>();
    // ── 事件 Events ────────────────────────────────────────────────────
    public event Action<ItemInstance> OnItemAdded;
    public event Action<ItemInstance> OnItemRemoved;
    public event Action OnInventoryChanged;

    // ── 只读属性 Read-only properties ──────────────────────────────────
    public int SlotCount => _slots.Count;
    public IReadOnlyList<ItemInstance> Slots => _slots;


    public StorageItemInstance(ItemBase data) : base(data)
    {
    }

    private int GetVolumeForItem(ItemInstance item)
    {
        return item.volume switch
        {
            ItemBase.ItemVolume.Tiny => ItemVolume.Volume_TINY,
            ItemBase.ItemVolume.Samll => ItemVolume.Volume_SMALL,
            ItemBase.ItemVolume.Normal => ItemVolume.Volume_NORMAL,
            ItemBase.ItemVolume.Bulky => int.MaxValue,
            ItemBase.ItemVolume.Huge => int.MaxValue,

            _ => int.MaxValue  // Bulky, Huge 无法放入
        };
    }

    public bool AddItem(ItemInstance item)
    {
        if (item == null)
        {
            Debug.LogWarning("[StorageItemInstance] AddItem: item 为 null。");
            return false;
        }

        // 检查是否可放入（Bulky/Huge 不能放入普通存储）
        int volumeNeeded = GetVolumeForItem(item);
        if (volumeNeeded == int.MaxValue)
        {
            Debug.Log($"[StorageItemInstance] {item.ItemName} 体积过大，无法放入");
            return false;
        }

        // 检查容量
        if (currentOccupiedVolume + volumeNeeded > capacity)
        {
            Debug.Log($"[StorageItemInstance] 无法添加 {item.ItemName}，超出容量");
            return false;
        }

        if (item.isBothHandsUsing)
        {
            item.isBothHandsUsing = false;
        }
        
        bool success;
        if (item is StackableItemInstance stackable)
            success = AddStackable(stackable);
        else
            success = AddSingle(item);

        if (success)
        {
            currentOccupiedVolume += volumeNeeded;
            Debug.Log($"[StorageItemInstance] 添加 {item.ItemName}，当前占用: {currentOccupiedVolume}/{capacity}");
        }

        return success;
    }

    private bool AddStackable(StackableItemInstance incoming)
    {
        int remaining = incoming.currentQuantity;

        // 1. 先合并进已有同类堆叠
        foreach (var slot in _slots)
        {
            if (remaining <= 0) break;

            if (slot is StackableItemInstance existing &&
                existing.itemData == incoming.itemData &&
                !existing.IsFull)
            {
                int absorbed = existing.AbsorbFrom(
                    new StackableItemInstance(incoming.itemData, remaining));
                remaining -= absorbed;
            }
        }

        // 2. 剩余量开新格
        while (remaining > 0)
        {
            int canFit = Mathf.Min(remaining, incoming.maxQuantity);
            var newSlot = new StackableItemInstance(incoming.itemData, canFit);
            remaining -= canFit;

            _slots.Add(newSlot);
            OnItemAdded?.Invoke(newSlot);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    private bool AddSingle(ItemInstance item)
    {
        _slots.Add(item);
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemInstance item)
    {
        bool removed = _slots.Remove(item);
        if (removed)
        {
            int volume = GetVolumeForItem(item);
            if (volume != int.MaxValue)
            {
                currentOccupiedVolume = Mathf.Max(0, currentOccupiedVolume - volume);
            }
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            Debug.Log($"[StorageItemInstance] 移除 {item.ItemName}，当前占用: {currentOccupiedVolume}/{capacity}");
        }
        return removed;
    }
        

    public void RemoveAllItem()
    {
        _slots.Clear();
        currentOccupiedVolume = 0;
    }

    public List<ItemInstance> GetAllItems()
        => new List<ItemInstance>(_slots);
}
