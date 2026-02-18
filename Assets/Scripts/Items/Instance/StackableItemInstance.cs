using System;
using UnityEngine;

[Serializable]
public class StackableItemInstance : ItemInstance, IStackable
{
    private int _currentQuantity;

    public int maxQuantity => itemData is IStackableData sd ? sd.MaxStackSize : 1;

    public int currentQuantity
    {
        get => _currentQuantity;
        set => _currentQuantity = Mathf.Clamp(value, 0, maxQuantity);
    }

    /// <summary>
    /// 创建一个堆叠物品实例。
    /// Creates a stackable item instance.
    /// </summary>
    /// <param name="data">必须实现 IStackableData 的 ScriptableObject。</param>
    /// <param name="quantity">初始数量，默认为最大值。</param>
    public StackableItemInstance(ItemBase data, int quantity = -1) : base(data)
    {
        if (data is not IStackableData)
            throw new ArgumentException($"{data.name} 未实现 IStackableData，不能创建 StackableItemInstance。");

        // 默认填满
        _currentQuantity = quantity < 0 ? maxQuantity : Mathf.Clamp(quantity, 0, maxQuantity);
    }

    public bool IsEmpty => currentQuantity <= 0;
    public bool IsFull => currentQuantity >= maxQuantity;
    public int FreeSpace => maxQuantity - currentQuantity;

    /// <summary>
    /// 消耗一个物品并返回行动，数量为 0 时返回 null。
    /// Consumes one item and returns the action; returns null when empty.
    /// </summary>
    public override ActionBase GetCurrentAction()
    {
        if (IsEmpty) return null;
        ActionBase action = base.GetCurrentAction();
        if (action != null) ItemConsume(); 
        return action;
    }

    /// <summary>
    /// 从另一个同类堆叠实例中转移数量，返回实际转移的数量。
    /// Transfers quantity from another stack of the same type. Returns the amount transferred.
    /// </summary>
    public int AbsorbFrom(StackableItemInstance other)
    {
        if (other.itemData != itemData) return 0;

        int canAbsorb = Mathf.Min(FreeSpace, other.currentQuantity);
        currentQuantity += canAbsorb;
        other.currentQuantity -= canAbsorb;
        return canAbsorb;
    }

    /// <summary>
    /// 拆分出指定数量为新的实例。
    /// Splits off a specified quantity into a new instance.
    /// </summary>
    public StackableItemInstance SplitStack(int amount)
    {
        amount = Mathf.Clamp(amount, 0, currentQuantity);
        currentQuantity -= amount;
        return new StackableItemInstance(itemData, amount);
    }

    public override ItemInstance CloneItemInstance()
    {
        return new StackableItemInstance(itemData, currentQuantity);
    }

    public bool ItemConsume()
    {
        currentQuantity--;
        return IsEmpty; // 返回 true 表示需要销毁
    }
}
