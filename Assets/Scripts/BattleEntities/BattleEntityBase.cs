using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BattleEntityBase : IBattleEntity
{
    public string memberName;
    public int currentHealth;
    public int maxHealth;
    public int currentAP { get; set; }
    public int maxAP;
    public int eachTurnRecoveredAP;
    public bool isPlayerFaction { get; set; }

    string IBattleEntity.memberName => memberName;
    int IBattleEntity.currentHealth { get => currentHealth; set => currentHealth = value; }
    int IBattleEntity.maxHealth => maxHealth;
    int IBattleEntity.maxAP => maxAP;

    public Dictionary<DamageType, ArmorStats> armorStats;

    public abstract void ExecuteAction(ActionBase battleAction, BattleEntityBase target);

    public void EntityTakeDamage(int damageAmount, DamageType type)
    {
        int finalDamage = damageAmount;

        if (armorStats.ContainsKey(type))
        {
            ArmorStats armor = armorStats[type];
            int armorReduction = Mathf.FloorToInt(
                (Mathf.Min(armor.armorValue, armor.armorStrength * damageAmount))
            );
            finalDamage = Mathf.Max(0, damageAmount - armorReduction);
        }

        EntitySetHealth(currentHealth - finalDamage); ;
    }

    public void EntitySetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, -800, maxHealth);
    }

    public bool EntityIsDead(PartyBattleEntity entity)
    {
        return entity.currentHealth <= entity.healthDead;
    }

    public void EntityRecoverAP(BattleEntityBase entity)
    {
        entity.currentAP = Mathf.Min(entity.maxAP, entity.currentAP + entity.eachTurnRecoveredAP);
    }
}
