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

    public bool isBothHandsUsing = false;

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

    public virtual int GetBaseDamage()
    {
        if(itemData is WeaponBase)
        {
            WeaponBase weapon = itemData as WeaponBase;
            if (weapon.allowUseOfBothHands && isBothHandsUsing)
            {
                return weapon.bothHandsUseDamage;
            }
            return weapon.GetBaseDamage();
        }
        return 0;
    }
    public virtual float GetBaseArmorPenetration()
    {
        if (itemData is WeaponBase)
        {
            WeaponBase weapon = itemData as WeaponBase;
            if(weapon.allowUseOfBothHands && isBothHandsUsing)
            {
                return weapon.bothHandsUseArmorPiercing;
            }
            return weapon.GetArmorPenetration();
        }
        return 0f;
    }

    public virtual void OnBothHandUse()
    {
        if(!(itemData is WeaponBase))
        {
            return;
        }
        var weapon = itemData as WeaponBase;
        if (!weapon.allowUseOfBothHands) { return; }
        ApplyBothHandBonus();
        Debug.Log("both hand");
    }
    public virtual void OnExitBothHandUse()
    {
        var weapon = itemData as WeaponBase;
        CancelBothHandBonus();
    }
    public virtual void ApplyBothHandBonus()
    {
        isBothHandsUsing = true;
    }
    public virtual void CancelBothHandBonus()
    {
        isBothHandsUsing = false;
    }
}
