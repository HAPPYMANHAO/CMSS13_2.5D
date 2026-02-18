using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Weapon")]
public class WeaponBase : HoldableBase, IEquippable
{
    [Header("Weapon Stats")]
    public DamageType primaryDamageType;
    public ActionType primaryActionType;

    [Header("Base Damage")]
    public int meleeDamage = 25;

    [Range(0f, 1f)] public float armorPenetration = 0f;


    [Header("Modifiers")]
    public List<DamageModifier> damageModifiers = new List<DamageModifier>();

    [SerializeField] public bool allowUseOfBothHands;
    [SerializeField] public Sprite holdInRightHandSprite;
    [SerializeField] public Sprite holdInLeftHandSprite;
    [SerializeField] public Sprite holdInBothHandsSprite;

    public int GetBaseDamage()
    {
        if (primaryActionType == ActionType.Melee)
        {
            return meleeDamage;
        }
        else if (primaryActionType == ActionType.Ranged)
        {
            RangedAction action = GetCurrentActions() as RangedAction;
            return action.projectileInfo.projectileDamage;
        }
        else
        {
            return 0;//无伤害，但是应该不会出现
        }
    }

    public float GetArmorPenetration()
    {
        if (primaryActionType == ActionType.Melee)
        {
            return armorPenetration;
        }
        else if (primaryActionType == ActionType.Ranged)
        {
            RangedAction action = GetCurrentActions() as RangedAction;
            return action.projectileInfo.projectileArmorPenetration;
        }
        else
        {
            return 0;//无伤害，但是应该不会出现
        }
    }


    public IEnumerable<DamageModifier> GetDamageModifiers(DamageType damageType)
    {
        foreach (var modifier in damageModifiers)
        {
            if (modifier.damageType == damageType) 
            {
                yield return modifier;
            }
        }
    }

    public override ActionBase GetCurrentActions()
    {
        if (providedActions == null || providedActions.Count == 0)
        {
            return null;
        }
        return providedActions.FirstOrDefault();//直接返回第一个行动，我们会有更好的办法的 TODO
    }

    public void OnEquip(CurrentPartyMemberInfo character)
    {
    }

    public void OnUnEquip(CurrentPartyMemberInfo character)
    {
    }

    public enum ActionType
    {
        Melee,         
        Ranged,         
    } 
}