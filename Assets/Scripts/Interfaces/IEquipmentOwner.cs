using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentOwner
{
    bool TryEquip(ItemInstance item);
    bool TryEquipArmor(ItemInstance item, EquipmentSlotType slot);
    bool TryEquipStorage(StorageItemInstance item, EquipmentSlotType slot);

    ItemInstance UnequipSlot(EquipmentSlotType slot);
    ItemInstance GetEquipped(EquipmentSlotType slot);
}
