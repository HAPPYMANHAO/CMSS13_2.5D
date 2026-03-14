using System;
using System.Collections.Generic;
using UnityEngine;

public class _Define : MonoBehaviour
{ 
}
//-----------------------常量const-----------------------//
public static class LogTextSettings
{
    public const int MAX_LOG_COUNT = 100;
}

public static class CustomInputString
{
    public const string ACTIVE_HOLD_ITEM = "ActiveHoldItem";
    public const string CHANGE_ACTIVE_HAND = "ChangeActiveHand";
}

public static class SceneName
{
    public const string OVER_WORLD = "OverworldScene";
    public const string BATTLE = "BattleScene";
}

//-----------------------枚举enum-----------------------//
//----------/成熟度 敌人的等级Maturity, Enemy's Level
public enum EnemyMaturityLevel
{
    Young,    //LV1
    Mature,   //LV2
    Elder,    //LV3
    Ancient,  //LV4
    Prime     //LV5
};

//----------/装甲类型ArmorType
public enum DamageType
{
    Melee,      // 近战
    Projectile, // 射弹
    Explosive,  // 爆炸
    Bio,        // 生化
    Fire        // 火焰
}

//----------/技能类型SkillType
public enum SkillType
{
    CQC,
    Endurance,
    FiremanCarry,
    Medical,
    Surgery,
    Construction,
    Engineering,
    JTAC,
    Leadership,
    Vehicles,
    Firearms,
    Powerloader,
    Melee,
    Research,
    SpecWeapon
}
//----------/阵营Faction
public enum Faction
{
    Survivor,//玩家阵营player faction
    Enemy,
    neutral,
}
//----------/战斗实体行动状态Battle Entity Action States
public enum BattleEntityActionStates
{
    Attack,            //实体的AI会积极进攻
    Defend,            //实体的AI会积极防守和治疗
    Run,               //实体的AI会积极尝试撤退
    Idle,              //实体的已经行动完成或未开始，在下一次回合开始前不能行动
    Unconscious,       //这个状态下的实体不能行动
}
//----------/Hands
public enum EntityHandsSlot { Left, Right }
//----------/Game State
public enum GameState
{
    Title,
    Overworld,
    Battle,
    Pause,
}
//----------/Buff Type
public enum BuffModifierType
{
    HealthChangeFlat,      // 每轮生命改变
    ArmorValueFlat,        // 护甲值加减
    ArmorIntegrityFlat,    // 护甲完整性加减
    DamageResistFlat,      // 伤害抗性加减
    DamageFlatBonus,       // 伤害加值（攻击者）
    DamagePercentBonus,    // 伤害百分比（攻击者）
    MaxAPFlat,             // 最大AP加减
    MaxAPPercent,          // 最大AP加减百分比
    CurrentAPFlat,         // 当前AP加减
    APRecoveryFlat,        // 每回合AP回复加减
    APRecoveryPercent,     // 每回合AP回复加减百分比
}
//----------/Equipment Type
public enum EquipmentSlotType
{
    Head,
    Mask,
    Body,
    Gloves,
    Shoes,
    Back,
    Belt,
    SuitStorage,
    PocketLeft,
    PocketRight,
}

//-----------------------class-----------------------//
//----------/包含装甲计算两个值（int和float）的类 class that contains two values (an int and a float) for armor calculation
[System.Serializable]
public class ArmorStats 
{
    public DamageType armorType;

    [SerializeField] public int armorValue;
    [SerializeField][Range(0f, 1f)] public float armorIntegrity;
}
//----------/包含伤害抗性的值的类
[System.Serializable]
public class DamageResistanceStats 
{
    public DamageType resistType;

    [SerializeField] public float damageResist;
}

//----------/包含计算血条的类 A class that includes the calculation of health bars
[System.Serializable]
public class HealthBarEntry
{
    public float healthThreshold;   // 例如 1.0(100%),0.63(63%),-0.5(-50%)
    public Sprite sprite;
}

//----------/包含技能数值的结构体 A struct containing skill values
[System.Serializable]
public struct InitialSkill
{
    public SkillType skillType;
    public int level;
}
//----------/包含AI自动行动的action的结构体，这是用于AI计算action权重的 A struct containing AI action ,This is used for calculating the action weights in AI.
[System.Serializable]
public struct AIActionWeight
{
    public ActionBase action;
    [Range(0, 100)] public int weight; // 如果是概率触发
}
//----------/包含AI自动行动的action的行动和目标数据,这个是发送给Turn manager的 A struct containing AI action and target, This is for sending to Turn Manager.
public struct DecisionAI
{
    public ActionBase action;
    public BattleEntityBase[] targets;

    // 辅助属性，方便判断是否有效
    public bool IsValid => action != null && targets != null && targets.Length > 0;
}
//-----------/装备槽 Equipment Slot
[System.Serializable]
public class EquipmentSlot
{
    public ItemInstance equipment;
}
//---------/Hand solt
public class HandSlot
{
    public ItemInstance item;

    public bool IsEmpty => item == null;

    public ActionBase GetCurrentAction()
    {
        return item?.GetCurrentAction();
    }
}
//---------/Buff
[System.Serializable]
public struct BuffModifier
{
    public BuffModifierType modType;
    public DamageType damageType;       // 只对特定伤害类型生效，None=全类型
    public float value;
}
//---------/Equipment Armor
[System.Serializable]
public class ArmorBonus 
{
    public ArmorStats armorStats = new ArmorStats(); // 确保初始化
    [Range(0f, 1f)] public float resistanceBonus;
}
