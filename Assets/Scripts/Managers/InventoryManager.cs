using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemBase> inventoryItems;

    public List<ItemBase> GetCurrentInventoryItems()
    {
        return inventoryItems;
    }

    public void AddItemToInventory(ItemBase item)
    {
        inventoryItems.Add(item);
    }
}
