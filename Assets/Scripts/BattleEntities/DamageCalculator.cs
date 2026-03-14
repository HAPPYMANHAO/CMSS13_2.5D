using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class DamageCalculator
{
    /// <summary>
    /// 标准伤害计算
    /// </summary>
    public static DamageResult CalculateDamage(
        BattleEntityBase user,
        BattleEntityBase target,
        DamageType damageType,
        ActionBase sourceAction = null)
    {
        int baseDamage = GetBaseDamage(user, damageType);
        float baseArmorPenetration = GetArmorPenetration(user);

        var modifiers = GetAllModifiers(user, damageType);
        int finalDamage = ApplyModifiers(baseDamage, modifiers, damageType);

        float totalPenetration = CalculateTotalPenetration(baseArmorPenetration, modifiers, damageType);
        int damageAfterArmor = CalculateArmorReduction(
            finalDamage,
            target,
            damageType,
            totalPenetration
        );

        int damageAfterResistance = CalculateDamageResistance(damageAfterArmor, target, damageType);

        return new DamageResult
        {
            baseDamage = baseDamage,
            finalDamage = damageAfterResistance,
            damageType = damageType,
            armorPenetration = totalPenetration,
        };
    }
    /// <summary>
    /// 用于BUFF等无user的场景
    /// </summary>
    public static DamageResult CalculateDamage(
        int damage,
        BattleEntityBase target,
        DamageType damageType,
        float armorPenetration = 0)
    {
        int baseDamage = damage;
        float baseArmorPenetration = armorPenetration;

        float totalPenetration = CalculateTotalPenetration(baseArmorPenetration, new List<DamageModifier>(), damageType);
        //new List<DamageModifier>()实际上就是空的，因为buff不需要计算它
        int damageAfterArmor = CalculateArmorReduction(
            baseDamage,
            target,
            damageType,
            totalPenetration
        );

        int damageAfterResistance = CalculateDamageResistance(damageAfterArmor, target, damageType);

        return new DamageResult
        {
            baseDamage = baseDamage,
            finalDamage = damageAfterResistance,
            damageType = damageType,
            armorPenetration = totalPenetration,
        };
    }

    private static int GetBaseDamage(BattleEntityBase attacker, DamageType damageType)
    {
        if (attacker is EnemyBattleEntity enemy)
        {
            switch (damageType)
            {
                case DamageType.Melee:
                    return enemy.meleeStrength;
                case DamageType.Projectile:
                    return enemy.rangedStrength;
                default:
                    return enemy.meleeStrength; // 默认使近战
            }
        }

        // 玩家使用装备
        if (attacker is PartyBattleEntity party)
        {
            var handItem = party.GetCurrentActiveHandItem();
            if (handItem is GunInstance gun)
            {
                Debug.Log(gun.GetCurrentProjectileInfo().name);
                return gun.GetCurrentProjectileInfo()?.projectileDamage ?? 0;
            }                       
            WeaponBase weapon = handItem?.itemData as WeaponBase;
            return weapon?.GetBaseDamage() ?? 5;//徒手伤害，但是目前没有实现，TODO
        }

        return 0;
    }

    private static float GetArmorPenetration(BattleEntityBase attacker)
    {
        if (attacker is EnemyBattleEntity enemy)
        {
            return enemy.armorPenetration;
        }

        // 玩家使用装备
        if (attacker is PartyBattleEntity party)
        {
            var handItem = party.GetCurrentActiveHandItem();
            if (handItem is GunInstance gun)
                return gun.GetCurrentProjectileInfo()?.projectileArmorPenetration ?? 0;

            WeaponBase equipment = handItem?.itemData as WeaponBase;
            if (equipment != null)
            {
                return equipment.GetArmorPenetration();
            }
        }

        return 0; //无穿甲
    }

    private static List<DamageModifier> GetAllModifiers(
        BattleEntityBase attacker,
        DamageType damageType)
    {
        List<DamageModifier> modifiers = new List<DamageModifier>();

        if (attacker is PartyBattleEntity party)
        {
            var handItem = party.GetCurrentActiveHandItem();
            WeaponBase equipment = handItem?.itemData as WeaponBase;
            if (equipment != null)
            {
                modifiers.AddRange(equipment.GetDamageModifiers(damageType));
            }
        }

        foreach (var buffMod in attacker.buffComponent.GetModifiers(BuffModifierType.DamageFlatBonus, damageType))
            modifiers.Add(new DamageModifier { flatBonus = (int)buffMod.value, damageType = damageType });
        foreach (var buffMod in attacker.buffComponent.GetModifiers(BuffModifierType.DamagePercentBonus, damageType))
            modifiers.Add(new DamageModifier { percentBonus = buffMod.value, damageType = damageType });

        return modifiers;
        // TODO
    }
    private static int ApplyModifiers(int baseDamage, List<DamageModifier> modifiers, DamageType damageType)
    {
        float finalDamage = baseDamage;

        List<DamageModifier> finalmodifiers =
            modifiers.Where(modifier => modifier.damageType == damageType).ToList();
        if (finalmodifiers != null && finalmodifiers.Count > 0)
        {
            int totalFlat = finalmodifiers.Sum(modifier => modifier.flatBonus);
            finalDamage += totalFlat;
            float totalPercent = finalmodifiers.Sum(modifier => modifier.percentBonus);
            finalDamage *= (1f + totalPercent);
        }
        return Mathf.RoundToInt(finalDamage);
    }


    private static float CalculateTotalPenetration(float basePenetration, List<DamageModifier> modifiers, DamageType damageType)
    {
        List<DamageModifier> finalmodifiers =
           modifiers.Where(modifier => modifier.damageType == damageType).ToList();
        if (finalmodifiers != null && finalmodifiers.Count > 0)
        {
            return finalmodifiers.Sum(m => m.armorPenetration) + basePenetration;
        }
        return basePenetration;
    }

    private static int CalculateDamageResistance(int damage, BattleEntityBase target, DamageType type)
    {
        if (!target.damageResistanceStats.ContainsKey(type))
            return damage;

        float effectivedamageResistance = Mathf.Clamp(
            target.damageResistanceStats[type],
            -2.5f,//DamageResistance最多允许伤害增加到250%
            1f
        );

        int resistanceReduction = Mathf.FloorToInt(effectivedamageResistance * damage);

        return Mathf.Max(0, damage - resistanceReduction);
    }


    private static int CalculateArmorReduction(
        int damage,
        BattleEntityBase target,
        DamageType type,
        float penetration)
    {
        if (!target.armorStats.ContainsKey(type))
            return damage;

        ArmorStats armor = target.armorStats[type];

        float effectiveIntegrity = Mathf.Clamp(
        armor.armorIntegrity - penetration,
        0f,
        1f
        );

        int effectiveArmor = Mathf.Clamp(
        armor.armorValue,
        0,
        int.MaxValue
        );

        int armorReduction = Mathf.FloorToInt(
            Mathf.Min(effectiveArmor, effectiveIntegrity * damage)
        );

        return Mathf.Max(0, damage - armorReduction);
    }
}

public struct DamageResult
{
    public int baseDamage;
    public int finalDamage;
    public DamageType damageType;
    public float armorPenetration;
    public bool wasCritical;
}
