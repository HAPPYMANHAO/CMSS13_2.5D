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
    public EntityAI entityAI;

    public event Action OnHealthChanged;
    public event Action OnApChanged;
    public event Action<BattleEntityBase> OnEntityDeath;
    public Faction entityFaction { get; set; }
    public BattleEntityActionStates entityBattleState { get; set; } = BattleEntityActionStates.Idle;

    string IBattleEntity.memberName => memberName;
    int IBattleEntity.currentHealth { get => currentHealth; set => currentHealth = value; }


    public Dictionary<DamageType, ArmorStats> armorStats;
    public Dictionary<DamageType, float> damageResistanceStats;

    public void ExecuteAction(ActionBase battleAction, BattleEntityBase[] target)
    {
        if (!battleAction.CanExecute(this, target))
        {
            return;
        }

        EntityConsumeAP(battleAction.costAP);

        battleAction.Execute(this, target);
    }

    public int EntityTakeDamage(BattleEntityBase user, DamageType damageType, ActionBase sourceAction = null)
    {
        DamageResult result = DamageCalculator.CalculateDamage(
            user,
            this,
            damageType,
            sourceAction
        );

        if (battleVisual != null)
        {
            battleVisual.PlayHurt();
        }

        EntitySetHealth(currentHealth - result.finalDamage);
        return result.finalDamage;
    }

    private void EntitySetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, healthDead, maxHealth);
        OnHealthChanged?.Invoke();

        if (EntityIsDead())
        {
            OnEntityDeath?.Invoke(this);
        }
    }

    public void EntityRevoverHealth(int recoverHealth)
    {
        EntitySetHealth(currentHealth + recoverHealth);
    }

    public bool EntityIsDead()
    {
        return currentHealth <= healthDead;
    }

    public void EntityRecoverAP(int recoverAP)
    {
        EntitySetAP(currentAP + recoverAP);
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
