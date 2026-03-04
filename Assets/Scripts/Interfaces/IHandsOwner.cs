using UnityEngine;

public interface IHandsOwner
{
    EntityHandsSlot currentActiveHand { get; set; }
    HandSlot leftHandEquipment { get; set; }
    HandSlot rightHandEquipment { get; set; }

    ItemInstance GetCurrentActiveHandItem();
    void GetItemFromToInventory(ItemInstance item);
    ItemInstance SentHoldItemToInventory();
}
