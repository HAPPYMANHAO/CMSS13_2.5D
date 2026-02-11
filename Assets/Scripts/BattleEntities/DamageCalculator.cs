using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

public static class DamageCalculator
{
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

        float totalPenetration = CalculateTotalPenetration(baseArmorPenetration ,modifiers ,damageType);
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
            WeaponBase equipment = party.GetCurrentActiveHandItem() as WeaponBase;
            if (equipment != null)
            {
                return equipment.GetBaseDamage();
            }
            return 5;//徒手伤害。TODO
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
            WeaponBase equipment = party.GetCurrentActiveHandItem() as WeaponBase;
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
            WeaponBase equipment = party.GetCurrentActiveHandItem() as WeaponBase;
            if (equipment != null)
            {
                modifiers.AddRange(equipment.GetDamageModifiers(damageType));
            }
        }
        // TODO

        return modifiers;
    }
    private static int ApplyModifiers(int baseDamage, List<DamageModifier> modifiers, DamageType damageType)
    {
        float finalDamage = baseDamage;

        List<DamageModifier> finalmodifiers = 
            new List<DamageModifier>().Where(modifier => modifier.damageType == damageType) 
            as List<DamageModifier>;
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
            new List<DamageModifier>().Where(modifier => modifier.damageType == damageType)
            as List<DamageModifier>;
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
            0f,
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

        int armorReduction = Mathf.FloorToInt(
            Mathf.Min(armor.armorValue, effectiveIntegrity * damage)
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
