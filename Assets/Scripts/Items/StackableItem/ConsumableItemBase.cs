using System.Linq;
using UnityEngine;

/// <summary>
/// 消耗品 ScriptableObject 定义模板。
/// Consumable item ScriptableObject definition template.
///
/// 修复：从 SO 中移除 IStackable（不再在资源上存储运行时数量），
///      改为实现 IStackableData 供 StackableItemInstance 读取上限。
/// Fix: Removed IStackable from SO (no longer storing runtime quantity on the asset).
///      Implements IStackableData so StackableItemInstance can read the max stack size.
///
/// 注意：GetCurrentActions() 不再递减数量——数量由 StackableItemInstance 管理。
/// Note: GetCurrentActions() no longer decrements quantity — managed by StackableItemInstance.
/// </summary>
[CreateAssetMenu(menuName = "Item/ConsumableItem")]
public class ConsumableItemBase : HoldableBase, IStackableData
{
    [Header("Stack Settings")]
    [SerializeField] private int _maxStackSize = 10;

    // IStackableData
    public int MaxStackSize => _maxStackSize;

    public override ItemInstance CreateInstance()
    {
        return new StackableItemInstance(this);
    }

    public override ActionBase GetCurrentActions()
    {
        if (providedActions == null || providedActions.Count == 0)
            return null;

        // 不再在这里递减数量！由 StackableItemInstance.GetCurrentAction() 负责。
        // Do NOT decrement quantity here! StackableItemInstance.GetCurrentAction() handles it.
        return providedActions.FirstOrDefault();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        isStackable = true;
        maxStackSize = _maxStackSize;
    }
#endif
}
