using System;
using UnityEngine;
[Serializable]
public class ItemInstance 
{
    public ItemBase itemData { get; protected set; }

    public readonly Guid instanceId;

    public ItemInstance(ItemBase data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data), "ItemInstance requires a valid ItemBase ScriptableObject.");

        itemData = data;
        instanceId = Guid.NewGuid();
    }

    public string ItemName => itemData.itemName;
    public Sprite Icon => itemData.icon;

    public virtual ActionBase GetCurrentAction()
    {
        if (itemData is HoldableBase holdable)
            return holdable.GetCurrentActions();
        return null;
    }

    public virtual ItemInstance CloneItemInstance()
    {
        return new ItemInstance(itemData);
    }
}
