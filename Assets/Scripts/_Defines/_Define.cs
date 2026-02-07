using UnityEngine;

public class _Define : MonoBehaviour
{ 
}
//-----------------------常量const-----------------------//
public static class LogTextSettings
{
    public const int MAX_LOG_COUNT = 100;
}

public static class LogText
{
    public const string LOG_VALUE = "[VALUE]";
    public const string LOG_TARGET = "[TARGET]";
    public const string LOG_USER = "[USER]";
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

//-----------------------结构体struct-----------------------//
//----------/包含装甲计算两个值（int和float）的结构体A struct that contains two values (an int and a float) for armor calculation
[System.Serializable]
public struct ArmorStats
{
    public DamageType armorType;
    public int armorValue;        // 护甲值armor value
    [Range(0f, 1f)]               //对于护甲来说，护甲抵消的伤害值为护甲值和护甲完整性*伤害值中的较小值
                                  //amount of damage absorbed by the armor is the smaller value between the armor value and the armor integrity * damage.
    public float armorIntegrity;   // 护甲强度（0%~100%）armor strength（0%~100%）
}

//----------/包含计算血条的结构体 A struct that includes the calculation of health bars
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
