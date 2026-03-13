using System;
using System.Collections.Generic;
using UnityEngine;

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
    public void Tick()
    {
        if (buffData.durationType == BuffBase.DurationType.Permanent || buffData.durationType == BuffBase.DurationType.Condition) return;
        remainingTurns--;
        if (remainingTurns <= 0)
            OnExpired?.Invoke(this);
    }

    // 叠加层数
    public void AddStack(BuffBase newBuff)
    {
        currentStacks = Mathf.Min(currentStacks + newBuff.baseStacks, buffData.maxStacks);
    }
    // 覆盖层数
    public void CoverStack(BuffBase newBuff)
    {
        currentStacks = newBuff.baseStacks;
    }
    // 如果新值更大，覆盖层数
    public void CoverStackWithLarger(BuffBase newBuff)
    {
        if(currentStacks <= newBuff.baseStacks)
        {
            CoverStack(newBuff);
        }
    }
    // 叠加持续回合数
    public void AddDuration(BuffBase newBuff)
    {
        remainingTurns = remainingTurns + newBuff.baseDuration;
    }
    // 覆盖持续回合数
    public void CoverDuration(BuffBase newBuff)
    {
        remainingTurns = newBuff.baseDuration;
    }
    // 如果新值更大，覆盖持续回合数
    public void CoverDurationWithLarger(BuffBase newBuff)
    {
        if (remainingTurns <= newBuff.baseDuration)
        {
            CoverDuration(newBuff);
        }
    }

    public IEnumerable<BuffModifier> GetModifiers() => buffData.modifiers;
}