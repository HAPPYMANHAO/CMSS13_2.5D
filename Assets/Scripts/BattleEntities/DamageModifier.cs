using System;
[Serializable]
public struct DamageModifier
{
    public DamageModifierType modifierType;
    public DamageType damageType;      // 伤害类型
    public int flatBonus;              // 固定加成 
    public float percentBonus;         // 百分比加成 
    public float armorPenetration;     // 护甲穿透

    public DamageModifier(DamageModifierType type, DamageType damageType,
                          int flat = 0, float percent = 0f, float penetration = 0f)
    {
        this.modifierType = type;
        this.damageType = damageType;
        this.flatBonus = flat;
        this.percentBonus = percent;
        this.armorPenetration = penetration;
    }
}

public enum DamageModifierType
{
    Equipment,      // 来自装备
    Skill,          // 来自技能
    Buff,           // 来自增益状态
    Debuff,         // 来自减益状态
}
