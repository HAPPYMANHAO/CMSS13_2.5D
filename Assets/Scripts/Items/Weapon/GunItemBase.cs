using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Gun")]
public class GunItemBase : WeaponBase
{
    public int apPerShot;
    [Header("Magazine")]
    public AmmoType acceptedAmmoType; // 只接受这种弹药
    public int magazineCapacity;      // 弹夹容量

    [Header("Fire Mode")]
    public List<FireMode> availableFireModes; // 这把枪支持哪些射击模式
    public int burstCount = 3;                // 爆发射击发数
    public float fireInterval = 0.25f;        // 两发之间的间隔（秒）

    public float baseAccuracy = 0;
    public int recoil = 0;

    public int reloadAmmoAP = 0;
    [Header("Both Hand")]
    public float bothHandAccuracyBonus = 0;// 直接增加精准度
    public float bothHandRecoilReduce = 0;// 百分比减少原有后坐力

    // 覆盖基类：枪械创建 GunInstance
    public override ItemInstance CreateInstance() => new GunInstance(this);

    public override ActionBase GetCurrentActions()
    {
        if (providedActions == null || providedActions.Count == 0)
        {
            return null;
        }
        ActionBase action = providedActions.FirstOrDefault();//直接返回第一个行动，我们会有更好的办法的 TODO
        return action;
    }
    //注意，枪械在DamageCalculator.cs读取枪械实例的投射物伤害，这两个方法在这里是无用的
    /*public override int GetBaseDamage()
    {
        
    }

    public override float GetArmorPenetration()
    {
        
    }*/
}
public enum FireMode { SemiAuto, FullAuto, Burst }