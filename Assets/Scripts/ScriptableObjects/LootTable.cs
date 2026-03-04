using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/LootTable")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> entries;

    public List<ItemInstance> GenerateLoot()
    {
        var result = new List<ItemInstance>();
        foreach (var entry in entries)
        {
            if (UnityEngine.Random.value <= entry.dropChance)
            {
                int qty = UnityEngine.Random.Range(entry.minQuantity, entry.maxQuantity + 1);

                if (entry.item.isStackable)
                {
                    // 堆叠物品
                    result.Add(new StackableItemInstance(entry.item, qty));
                }
                else
                {
                    // 非堆叠物品
                    for (int i = 0; i < qty; i++)
                    {
                        result.Add(entry.item.CreateInstance());
                    }
                }
            }
        }
        return result;
    }
}

[Serializable]
public class LootEntry
{
    public ItemBase item;
    [Range(0f, 1f)] public float dropChance = 1f;
    public int minQuantity = 1;
    public int maxQuantity = 1;
}
