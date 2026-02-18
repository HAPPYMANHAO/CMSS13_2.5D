using UnityEngine;
///运行时可堆叠物品实例的接口。Interface for runtime stackable item instances.
public interface IStackable
{
    /// <summary>最大堆叠数量（从 SO 读取）。Max stack size (read from SO).</summary>
    int maxQuantity { get; }

    /// <summary>当前堆叠数量（运行时可变）。Current stack quantity (mutable at runtime).</summary>
    int currentQuantity { get; set; }
}