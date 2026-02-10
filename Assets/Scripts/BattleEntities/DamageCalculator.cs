using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DamageCalculator
{
    public static DamageResult CalculateDamage(
        BattleEntityBase user,
        BattleEntityBase target,
        DamageType damageType,
        ActionBase sourceAction = null)
    {
        int baseDamage = GetBaseDamage(user, damageType);

        var modifiers = GetAllModifiers(user, damageType);
        int finalDamage = ApplyModifiers(baseDamage, modifiers);

        float totalPenetration = CalculateTotalPenetration(modifiers);
        int damageAfterArmor = CalculateArmorReduction(
            finalDamage,
            target,
            damageType,
            totalPenetration
        );

        return new DamageResult
        {
            baseDamage = baseDamage,
            finalDamage = damageAfterArmor,
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

        // 玩家使用装备提供伤害
        if (attacker is PartyBattleEntity party)
        {
            WeaponBase equipment = party.GetCurrentActiveHand() as WeaponBase; 
            if (equipment != null)
            {
                return equipment.GetBaseDamage();
            }
            return 5; 
        }

        return 0;
    }

    private static List<DamageModifier> GetAllModifiers(
        BattleEntityBase attacker,
        DamageType damageType)
    {
        List<DamageModifier> modifiers = new List<DamageModifier>();

        if (attacker is PartyBattleEntity party)
        {
            WeaponBase equipment = party.GetCurrentActiveHand() as WeaponBase;
            if (equipment != null)
            {
                modifiers.AddRange(equipment.GetDamageModifiers(damageType));
            }
        }
        // TODO

        return modifiers;
    }
    private static int ApplyModifiers(int baseDamage, List<DamageModifier> modifiers)
    {
        float finalDamage = baseDamage;

        int totalFlat = modifiers.Sum(modifier => modifier.flatBonus);
        finalDamage += totalFlat;


        float totalPercent = modifiers.Sum(modifier => modifier.percentBonus);
        finalDamage *= (1f + totalPercent);

        return Mathf.RoundToInt(finalDamage);
    }


    private static float CalculateTotalPenetration(List<DamageModifier> modifiers)
    {
        return modifiers.Sum(m => m.armorPenetration);
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
