using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Cinemachine.CinemachineFreeLookModifier;
using static UnityEngine.UI.GridLayoutGroup;

public class BuffInstance
{
    public BuffBase buffData { get; private set; }
    public int remainingTurns;
    public int currentStacks;

    public event Action<BuffInstance> OnExpired;

    public BuffInstance(BuffBase data, int stacks)
    {
        buffData = data;
        remainingTurns = data.baseDuration;
        currentStacks = Mathf.Clamp(stacks, 1, data.maxStacks);
    }

    // 回合结束时调用
    public void Tick(BattleEntityBase owner)
    {
        if (buffData.durationType == BuffBase.DurationType.Permanent || buffData.durationType == BuffBase.DurationType.Condition) return;
        remainingTurns--;
        OnTick(owner, this);
        if (remainingTurns <= 0)
            OnExpired?.Invoke(this);
    }

    public virtual void OnApply(BattleEntityBase target, BuffInstance buff)
    {
        foreach (BuffModifier buffModifier in buff.buffData.modifiers)
        {
            switch (buffModifier.modType)
            {
                case BuffModifierType.ArmorValueFlat:
                    ArmorValueFlatApply(target, buff, buffModifier);
                    break;
                case BuffModifierType.ArmorIntegrityFlat:
                    ArmorIntegrityFlatApply(target, buff, buffModifier);
                    break;
                case BuffModifierType.DamageResistFlat:
                    break;
                case BuffModifierType.DamageFlatBonus:
                    break;
                case BuffModifierType.DamagePercentBonus:
                    break;
                case BuffModifierType.MaxAPFlat:
                    break;
                case BuffModifierType.MaxAPPercent:
                    break;
                case BuffModifierType.APRecoveryFlat:
                    break;
                case BuffModifierType.APRecoveryPercent:
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void OnTick(BattleEntityBase target, BuffInstance buff)
    {
        foreach (BuffModifier buffModifier in buff.buffData.modifiers)
        {
            switch (buffModifier.modType)
            {
                case BuffModifierType.HealthChangeFlat:
                    HealthChangeFlatBuffTick(target, buff, buffModifier);
                    break;
                case BuffModifierType.CurrentAPFlat:
                    //TODO
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void OnRemove(BattleEntityBase target, BuffInstance buff)
    {
        foreach (BuffModifier buffModifier in buff.buffData.modifiers)
        {
            switch (buffModifier.modType)
            {
                case BuffModifierType.ArmorValueFlat:
                    ArmorValueFlatRemove(target, buff, buffModifier);
                    break;
                case BuffModifierType.ArmorIntegrityFlat:
                    ArmorIntegrityFlatRemove(target, buff, buffModifier);
                    break;
                case BuffModifierType.DamageResistFlat:
                    break;
                case BuffModifierType.DamageFlatBonus:
                    break;
                case BuffModifierType.DamagePercentBonus:
                    break;
                case BuffModifierType.MaxAPFlat:
                    break;
                case BuffModifierType.MaxAPPercent:
                    break;
                case BuffModifierType.APRecoveryFlat:
                    break;
                case BuffModifierType.APRecoveryPercent:
                    break;
                default:
                    break;
            }
        }
    }

    public virtual float GetStatModifier(BuffModifier type, DamageType? damageType = null) => 0f;

    // 叠加层数
    public void AddStack(BattleEntityBase target, BuffBase newBuff)
    {
        currentStacks = Mathf.Min(currentStacks + newBuff.baseStacks, buffData.maxStacks);
        foreach (BuffModifier buffModifier in newBuff.modifiers)
        {
            RefreshBuff(target, buffModifier);
        }
    }
    // 覆盖层数
    public void CoverStack(BattleEntityBase target, BuffBase newBuff)
    {
        currentStacks = Mathf.Min(newBuff.baseStacks, buffData.maxStacks);
    }
    // 如果新值更大，覆盖层数
    public void CoverStackWithLarger(BattleEntityBase target, BuffBase newBuff)
    {
        if (currentStacks <= newBuff.baseStacks)
        {
            CoverStack(target, newBuff);
        }
    }
    // 叠加持续回合数
    public void AddDuration(BattleEntityBase target, BuffBase newBuff)
    {
        remainingTurns = Mathf.Min(remainingTurns + newBuff.baseDuration, buffData.maxDuration);
    }
    // 覆盖持续回合数
    public void CoverDuration(BattleEntityBase target, BuffBase newBuff)
    {
        remainingTurns = Mathf.Min(newBuff.baseDuration, buffData.maxDuration);
    }
    // 如果新值更大，覆盖持续回合数
    public void CoverDurationWithLarger(BattleEntityBase target, BuffBase newBuff)
    {
        if (remainingTurns <= newBuff.baseDuration)
        {
            CoverDuration(target, newBuff);
        }
    }

    private void HealthChangeFlatBuffTick(BattleEntityBase target, BuffInstance buff, BuffModifier buffModifier)
    {
        if (!buff.buffData.isHealthChangeDamage)
        {
            // 使用 modifier.value * stacks
            int healAmount = (int)(buffModifier.value * buff.currentStacks);
            target.EntityRevoverHealth(healAmount);
        }
        else
        {
            // 使用 modifier.value * stacks(在TakeDamageFromBuff函数内)
            target.EntityTakeDamageFromBuff(buff, buffModifier);
        }
    }

    private void RefreshBuff(BattleEntityBase target, BuffModifier buffModifier)
    {
        switch (buffModifier.modType)
        {
            case BuffModifierType.ArmorValueFlat:
                ArmorValueFlatRemove(target, this, buffModifier);
                ArmorIntegrityFlatApply(target, this, buffModifier);
                break;
            case BuffModifierType.ArmorIntegrityFlat:
                ArmorIntegrityFlatRemove(target, this, buffModifier);
                ArmorIntegrityFlatApply(target, this, buffModifier);
                break;
            case BuffModifierType.DamageResistFlat:
                break;
            case BuffModifierType.DamageFlatBonus:
                break;
            case BuffModifierType.DamagePercentBonus:
                break;
            case BuffModifierType.MaxAPFlat:
                break;
            case BuffModifierType.MaxAPPercent:
                break;
            case BuffModifierType.APRecoveryFlat:
                break;
            case BuffModifierType.APRecoveryPercent:
                break;
            default:
                break;
        }

    }

    private void ArmorValueFlatApply(BattleEntityBase target, BuffInstance buff, BuffModifier buffModifier)
    {
        int armorValueBouns = (int)(buff.currentStacks * buffModifier.value);
        target.armorStats[buffModifier.damageType].armorValue += armorValueBouns;
    }
    private void ArmorValueFlatRemove(BattleEntityBase target, BuffInstance buff, BuffModifier buffModifier)
    {
        int armorValueBouns = (int)(buff.currentStacks * buffModifier.value);
        target.armorStats[buffModifier.damageType].armorValue -= armorValueBouns;
    }

    private void ArmorIntegrityFlatApply(BattleEntityBase target, BuffInstance buff, BuffModifier buffModifier)
    {
        float armorIntegrityBouns = buff.currentStacks * buffModifier.value;
        target.armorStats[buffModifier.damageType].armorIntegrity += armorIntegrityBouns;
    }
    private void ArmorIntegrityFlatRemove(BattleEntityBase target, BuffInstance buff, BuffModifier buffModifier)
    {
        float armorIntegrityBouns = buff.currentStacks * buffModifier.value;
        target.armorStats[buffModifier.damageType].armorIntegrity -= armorIntegrityBouns;
    }

    public IEnumerable<BuffModifier> GetModifiers() => buffData.modifiers;
}