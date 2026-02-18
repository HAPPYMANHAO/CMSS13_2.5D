using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;
    [SerializeField][TextArea(1, 3)] public string description;

    [Header("Inventory")]
    [Tooltip("此物品是否可以堆叠存放？ Can this item stack in inventory?")]
    [SerializeField] public bool isStackable = false;

    [Tooltip("最大堆叠数量（仅当 isStackable 为 true 时有效）。Max stack size (only when isStackable is true).")]
    [SerializeField] public int maxStackSize = 1;

    public virtual ItemInstance CreateInstance()
    {
        if (isStackable && this is IStackableData)
            return new StackableItemInstance(this);

        return new ItemInstance(this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!isStackable)
            maxStackSize = 1;
        else
            maxStackSize = Mathf.Max(1, maxStackSize);
    }
#endif
}
