using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Buff")]
public class BuffBase : ScriptableObject
{
    public string buffName;
    public Sprite icon;
    public bool isHealthChangeDamage = false;//为health change buff使用，false为恢复生命，true为造成伤害
    //如果是恢复，则modifiers伤害类型是无用的

    public enum StackAddType { Add, Cover, None }
    public enum DurationAddType { Add, Cover, None }
    public enum DurationType { Turns, condition, Permanent }
    public DurationType durationType;
    public StackAddType stackAddType;
    public DurationAddType durationAddType;
    public int baseDuration = 2;     // 持续回合数
    public int baseStacks = 1;       // 基础叠层
    public int maxStacks = 99;       // 最大叠层

    public List<BuffModifier> modifiers;  //提供数据
}
