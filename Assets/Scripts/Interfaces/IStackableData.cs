using UnityEngine;

public interface IStackableData
{
    /// <summary>此物品类型的最大堆叠数量。 Maximum stack size for this item type.</summary>
    /// 仅用于SO
    int MaxStackSize { get; }
}
