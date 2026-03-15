using System.Collections.Generic;
using UnityEngine;

public abstract class ArmorItemBase : ItemBase, IEquippable
{
    [Header("Equipment")]
    public EquipmentSlotType slotType; // 这件装备属于哪个槽

    [Header("Armor Bonuses")]
    public List<ArmorBonus> armorBonuses; // 装备后叠加的护甲加成
    [Range(0f, 1f)] public float resistanceBonusFlat; // 通用伤害抗性加成

    // 装备时把加成写入角色
    public void OnEquip(CurrentPartyMemberInfo member)
    {
        foreach (var bonus in armorBonuses)
        {
            if (member.armorStats.ContainsKey(bonus.armorStats.armorType))
            {
                // 修改现有护甲
                var existing = member.armorStats[bonus.armorStats.armorType];
                existing.armorValue = Mathf.Max(0, existing.armorValue += bonus.armorStats.armorValue);
                existing.armorIntegrity = Mathf.Clamp01(existing.armorIntegrity += bonus.armorStats.armorIntegrity);
                member.armorStats[bonus.armorStats.armorType] = existing;
            }
            else
            {
                // 新增护甲类型
                member.armorStats[bonus.armorStats.armorType] = new ArmorStats
                {
                    armorType = bonus.armorStats.armorType,
                    armorValue = bonus.armorStats.armorValue,
                    armorIntegrity = bonus.armorStats.armorIntegrity,
                };
            }

            // 伤害抗性叠加
            if (member.damageResistanceStats.ContainsKey(bonus.armorStats.armorType))
                member.damageResistanceStats[bonus.armorStats.armorType] =
                    Mathf.Clamp01(member.damageResistanceStats[bonus.armorStats.armorType] += bonus.resistanceBonus);
            else
                member.damageResistanceStats[bonus.armorStats.armorType] = bonus.resistanceBonus;
        }

        OnEquipEffect(member); // 子类可以覆盖添加额外效果
    }

    // 卸下时撤销加成
    public void OnUnEquip(CurrentPartyMemberInfo member)
    {
        foreach (var bonus in armorBonuses)
        {
            if (member.armorStats.ContainsKey(bonus.armorStats.armorType))
            {
                var existing = member.armorStats[bonus.armorStats.armorType];
                existing.armorValue = Mathf.Clamp(existing.armorValue -= bonus.armorStats.armorValue, 0, int.MaxValue);
                existing.armorIntegrity = Mathf.Clamp(existing.armorIntegrity -= bonus.armorStats.armorIntegrity, 0f, 1f);
                member.armorStats[bonus.armorStats.armorType] = existing;
            }

            if (member.damageResistanceStats.ContainsKey(bonus.armorStats.armorType))
                member.damageResistanceStats[bonus.armorStats.armorType] =
                    Mathf.Clamp01(member.damageResistanceStats[bonus.armorStats.armorType] -= bonus.resistanceBonus);
        }

        OnUnEquipEffect(member);
    }

    // 子类覆盖：特殊装备效果
    protected virtual void OnEquipEffect(CurrentPartyMemberInfo member) { }
    protected virtual void OnUnEquipEffect(CurrentPartyMemberInfo member) { }

    // ArmorItem 不可堆叠
    public override ItemInstance CreateInstance() => new ItemInstance(this);
}
