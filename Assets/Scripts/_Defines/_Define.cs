using UnityEngine;

public class _Define : MonoBehaviour
{ 
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

//-----------------------结构体struct-----------------------//
//----------/包含装甲计算两个值（int和float）的结构体A struct that contains two values (an int and a float) for armor calculation
[System.Serializable]
public struct ArmorStats
{
    public DamageType armorType;
    public int armorValue;        // 护甲值armor value
    [Range(0f, 1f)]
    public float armorStrength;   // 护甲强度（0%~100%）armor strength（0%~100%）
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
