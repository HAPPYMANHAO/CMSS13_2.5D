using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStorage
{
    public bool AddItem(ItemInstance item);
    public bool RemoveItem(ItemInstance item);
    public List<ItemInstance> GetAllItems();

    // ── 事件 Events ────────────────────────────────────────────────────
    // 改为实例事件，每个存储容器有自己的事件
    public event Action<ItemInstance> OnItemAdded;
    public event Action<ItemInstance> OnItemRemoved;
    public event Action OnInventoryChanged;
}
