using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public BuffComponent buffComponent = new BuffComponent();

    public void ExecuteAction(ActionBase battleAction, BattleEntityBase[] target)
    {
        if (!battleAction.CanExecute(this, target))
        {
            return;
        }

        int actualCost = battleAction.GetCostAP(this);
        EntityConsumeAP(actualCost);

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

    public int EntityTakeDamageFromBuff(BuffInstance buff, BuffModifier buffModifier)
    {
        int damageAmount = (int)(buffModifier.value * buff.currentStacks);

        DamageResult result = DamageCalculator.CalculateDamage(
            damageAmount,
            this,
            buffModifier.damageType
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

public class BuffComponent
{
    private readonly List<BuffInstance> _activeBuffs = new List<BuffInstance>();
    public IReadOnlyList<BuffInstance> ActiveBuffs => _activeBuffs;

    public event Action OnBuffChanged; // GUI

    public BattleEntityBase owner;

    public void AddBuff(BuffBase buffData)
    {
        // 检查是否已有同类Buff（叠加或刷新）
        var existing = _activeBuffs.FirstOrDefault(b => b.buffData == buffData);
        if (existing != null)
        {
            switch (buffData.stackAddType)
            {
                case BuffBase.StackAddType.Add:
                    existing.AddStack(owner,buffData);
                    break;
                case BuffBase.StackAddType.Cover:
                    existing.CoverStack(owner, buffData);
                    break;
                case BuffBase.StackAddType.CoverWithLarger:
                    existing.CoverStackWithLarger(owner, buffData);
                    break;
                case BuffBase.StackAddType.None:
                    break;
                default:
                    break;
            }

            switch (buffData.durationAddType)
            {
                case BuffBase.DurationAddType.Add:
                    existing.AddDuration(owner, buffData);
                    break;
                case BuffBase.DurationAddType.Cover:
                    existing.CoverDuration(owner, buffData);
                    break;
                case BuffBase.DurationAddType.CoverWithLarger:
                    existing.CoverDurationWithLarger(owner, buffData);
                    break;
                case BuffBase.DurationAddType.None:
                    break;
                default:
                    break;
            }

        }

        else
        {
            var newBuff = new BuffInstance(buffData, buffData.baseStacks);
            newBuff.OnExpired += HandleBuffExpired;
            newBuff.OnApply(owner, newBuff);
            _activeBuffs.Add(newBuff);
        }
        OnBuffChanged?.Invoke();
    }

    public void RemoveBuff(BuffBase buffData)
    {
        var buff = _activeBuffs.FirstOrDefault(b => b.buffData == buffData);
        if (buff != null)
        {
            HandleBuffExpired(buff);
        }
    }

    // 回合结束时由 TurnManager 调用
    public void TickAllBuffsEndTurn(BattleEntityBase owner)
    {
        foreach (var buff in _activeBuffs.ToList())
            if (buff.buffData.triggeringTime == BuffBase.TriggeringTime.EndTurn) buff.Tick(owner);
        OnBuffChanged?.Invoke();
    }
    public void TickAllBuffsStartTurn(BattleEntityBase owner)
    {
        foreach (var buff in _activeBuffs.ToList())
            if (buff.buffData.triggeringTime == BuffBase.TriggeringTime.StartTurn) buff.Tick(owner);
        OnBuffChanged?.Invoke();
    }

    private void HandleBuffExpired(BuffInstance buff)
    {
        buff.OnRemove(owner, buff);
        _activeBuffs.Remove(buff);
        OnBuffChanged?.Invoke();
    }

    // DamageCalculator 用：取出指定类型的所有修改器
    public IEnumerable<BuffModifier> GetModifiers(BuffModifierType modType, DamageType? damageType = null)
    {
        foreach (var buff in _activeBuffs)
            foreach (var mod in buff.GetModifiers())
            {
                if (mod.modType != modType) continue;
                if (damageType.HasValue && mod.damageType != damageType.Value) continue;
                // 返回按叠层倍增的值
                yield return new BuffModifier
                {
                    modType = mod.modType,
                    damageType = mod.damageType,
                    value = mod.value * buff.currentStacks
                };
            }
    }
}