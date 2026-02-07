using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BattleEntityBase : IBattleEntity
{
    public string memberName;
    public int currentHealth;
    public int maxHealth { get; set; }
    public int healthDead { get; set; }
    public int currentAP { get; set; }
    public int maxAP { get; set; }
    public int eachTurnRecoveredAP;

    public CharacterBattleVisual battleVisual;

    public event Action OnHealthChanged;
    public event Action OnApChanged;
    public event Action OnEntityDeath;
    public Faction entityFaction { get; set; }
    public BattleEntityActionStates entityBattleState { get; set; } = BattleEntityActionStates.Idle;

    string IBattleEntity.memberName => memberName;
    int IBattleEntity.currentHealth { get => currentHealth; set => currentHealth = value; }


    public Dictionary<DamageType, ArmorStats> armorStats;

    public void ExecuteAction(ActionBase battleAction, BattleEntityBase[] target)
    {
        if (!battleAction.CanExecute(this, target))
        {
            return;
        }

        EntityConsumeAP(battleAction.costAP);

        battleAction.Execute(this, target);
    }

    public void EntityTakeDamage(int damageAmount, DamageType type, float armourPenetration)
    {
        int finalDamage = CalculateArmorReduction(damageAmount, type, armourPenetration);
        if (battleVisual != null)
        {
            battleVisual.PlayHurt();
        }
        Debug.Log(this.memberName + "受到伤害：" + finalDamage + "剩下血量为:" + this.currentHealth);
        EntitySetHealth(currentHealth - finalDamage);
    }

    private int CalculateArmorReduction(int damageAmount, DamageType type, float armourPenetration)
    {
        int finalDamage = damageAmount;
        if (armorStats.ContainsKey(type))
        {
            ArmorStats armor = armorStats[type];

            float currentArmorIntegrity = Math.Clamp(armor.armorIntegrity - armourPenetration, 0f, 1f);
            int armorReduction = Mathf.FloorToInt(
                (Mathf.Min(armor.armorValue, currentArmorIntegrity * damageAmount))
            );
            return finalDamage = Mathf.Max(0, damageAmount - armorReduction);
        }

        return damageAmount;
    }

    private void EntitySetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, healthDead, maxHealth);
        OnHealthChanged?.Invoke();

        if (EntityIsDead())
        {
            OnEntityDeath.Invoke();
        }
    }

    public bool EntityIsDead()
    {
        return currentHealth <= healthDead;
    }

    public void EntityRecoverAP()
    {
        currentAP = Mathf.Min(maxAP, currentAP + eachTurnRecoveredAP);
    }

    public void EntityConsumeAP(int consumedAP)
    {
        EntitySetAP(currentAP - consumedAP);
    }

    private void EntitySetAP(int newAP)
    {
        currentAP = Mathf.Clamp(newAP, 0, maxAP);
        OnApChanged?.Invoke();
    }
}
