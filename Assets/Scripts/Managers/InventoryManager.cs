using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 50;

    // 库存槽列表，每格是一个 ItemInstance（可堆叠或不可堆叠）
    private readonly List<ItemInstance> _slots = new List<ItemInstance>();

    // ── 事件 Events ────────────────────────────────────────────────────
    public static event Action<ItemInstance> OnItemAdded;
    public static event Action<ItemInstance> OnItemRemoved;
    public static event Action OnInventoryChanged;

    // ── 只读属性 Read-only properties ──────────────────────────────────
    public int SlotCount => _slots.Count;
    public bool IsFull => _slots.Count >= maxSlots;
    public IReadOnlyList<ItemInstance> Slots => _slots;
    // ── test ──────────────────────────────────
    [SerializeField] List<ItemBase> testItems;

    private void Start()
    {
        foreach (var item in testItems) 
        {
            AddItemFromData(item);
        }   
    }

    // ── 添加物品 Add Item ───────────────────────────────────────────────

    /// <summary>
    /// 添加一个物品实例到库存。
    /// Adds an item instance to the inventory.
    ///
    /// 可堆叠物品会先尝试合并到现有堆叠，溢出时开新格。
    /// Stackable items first try to merge into existing stacks; overflow opens a new slot.
    /// </summary>
    /// <returns>成功添加返回 true；库存已满且无法合并时返回 false。</returns>
    public bool AddItem(ItemInstance item)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventoryManager] AddItem: item 为 null。");
            return false;
        }

        if (item is StackableItemInstance stackable)
            return AddStackable(stackable);

        return AddSingle(item);
    }

    /// <summary>
    /// 从 SO 模板创建实例并添加到库存（便捷方法）。
    /// Convenience: creates an instance from an SO template and adds it.
    /// </summary>
    public bool AddItemFromData(ItemBase itemData, int quantity = 10)
    {
        if (itemData == null) return false;

        if (itemData.isStackable && itemData is IStackableData)
        {
            var stack = new StackableItemInstance(itemData, quantity);
            return AddItem(stack);
        }

        // 非堆叠：每个单独加入
        bool allAdded = true;
        for (int i = 0; i < quantity; i++)
        {
            if (!AddItem(itemData.CreateInstance()))
                allAdded = false;
        }
        return allAdded;
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
            if (_slots.Count >= maxSlots)
            {
                return false;
            }

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
        if (_slots.Count >= maxSlots)
        {
            return false;
        }

        _slots.Add(item);
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    // ── 移除物品 Remove Item ───────────────────────────────────────────

    /// <summary>
    /// 移除指定实例（精确匹配 instanceId）。
    /// Removes the exact instance (matches by instanceId).
    /// </summary>
    public bool RemoveItem(ItemInstance item)
    {
        if (item == null) return false;

        int index = _slots.FindIndex(s => s.instanceId == item.instanceId);
        if (index < 0) return false;

        _slots.RemoveAt(index);

        OnItemRemoved?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// 从库存中消耗指定 SO 类型的若干数量。
    /// Consumes a specified quantity of an item type from inventory.
    /// 先从最少量的堆叠开始消耗（最优化格位使用）。
    /// Consumes from stacks with least quantity first (optimizes slot usage).
    /// </summary>
    public bool ConsumeItem(ItemBase itemData, int quantity = 1)
    {
        int available = GetTotalQuantity(itemData);
        if (available < quantity) return false;

        int remaining = quantity;

        // 从量少的格开始消耗
        var stacks = _slots
            .OfType<StackableItemInstance>()
            .Where(s => s.itemData == itemData)
            .OrderBy(s => s.currentQuantity)
            .ToList();

        foreach (var stack in stacks)
        {
            if (remaining <= 0) break;

            int take = Mathf.Min(stack.currentQuantity, remaining);
            stack.currentQuantity -= take;
            remaining -= take;

            if (stack.IsEmpty)
                RemoveItem(stack);
        }

        if (remaining == 0)
            OnInventoryChanged?.Invoke();

        return remaining == 0;
    }

    // ── 查询 Queries ───────────────────────────────────────────────────

    /// <summary>
    /// 获取指定 SO 类型的物品的全部数量（跨所有堆叠）。
    /// Returns total quantity across all stacks of a given item type.
    /// </summary>
    public int GetTotalQuantity(ItemBase itemData)
    {
        return _slots
            .OfType<StackableItemInstance>()
            .Where(s => s.itemData == itemData)
            .Sum(s => s.currentQuantity);
    }

    /// <summary>
    /// 检查库存中是否至少有 minCount 个指定物品。
    /// Checks whether inventory has at least minCount of the given item.
    /// </summary>
    public bool HasItem(ItemBase itemData, int minCount = 1)
    {
        // 可堆叠物品：按数量统计
        if (itemData.isStackable)
            return GetTotalQuantity(itemData) >= minCount;

        // 非堆叠：按格数统计
        return _slots.Count(s => s.itemData == itemData) >= minCount;
    }

    /// <summary>
    /// 返回所有格位的副本列表（GUI 刷新用）。
    /// Returns a copy of all slots (for GUI refresh).
    /// </summary>
    public List<ItemInstance> GetAllItems()
    {
        return new List<ItemInstance>(_slots);
    }

    /// <summary>
    /// 通过 instanceId 查找格位。
    /// Finds a slot by instanceId.
    /// </summary>
    public ItemInstance FindById(Guid instanceId)
    {
        return _slots.FirstOrDefault(s => s.instanceId == instanceId);
    }

    /// <summary>
    /// 查找所有匹配指定 SO 的格位。
    /// Finds all slots matching a given SO template.
    /// </summary>
    public List<ItemInstance> FindAllByData(ItemBase itemData)
    {
        return _slots.Where(s => s.itemData == itemData).ToList();
    }
}
