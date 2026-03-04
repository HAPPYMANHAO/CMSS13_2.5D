using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/StorageItem")]
public class StorageItemBase : ItemBase
{
    [Header("Storage")]
    public EquipmentSlotType slotType; // 属于哪个槽（Back/Belt/Pocket）
    public int storageCapacity;        // 提供几个存储格

    public override ItemInstance CreateInstance()
        => new StorageItemInstance(this);
}

// StorageItemInstance.cs — 运行时实例，持有实际存储的物品
public class StorageItemInstance : ItemInstance
{
    private List<ItemInstance> _storedItems;
    private StorageItemBase StorageData => itemData as StorageItemBase;

    public IReadOnlyList<ItemInstance> StoredItems => _storedItems;
    public int Capacity => StorageData.storageCapacity;
    public bool IsFull => _storedItems.Count >= Capacity;

    public StorageItemInstance(ItemBase data) : base(data)
    {
        _storedItems = new List<ItemInstance>();
    }

    public bool AddItem(ItemInstance item)
    {
        if (IsFull) return false;
        _storedItems.Add(item);
        return true;
    }

    public bool RemoveItem(ItemInstance item)
        => _storedItems.Remove(item);

    public void RemoveAllItem()
    {

        _storedItems.Clear();

    }

    public List<ItemInstance> GetAllItems()
        => new List<ItemInstance>(_storedItems);
}
