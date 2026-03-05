using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Gun")]
public class GunItemBase : WeaponBase
{
    public int costAP;
    [Header("Magazine")]
    public AmmoType acceptedAmmoType; // 只接受这种弹药
    public int magazineCapacity;      // 弹夹容量

    [Header("Fire Mode")]
    public List<FireMode> availableFireModes; // 这把枪支持哪些射击模式
    public int burstCount = 3;                // 爆发射击发数
    public float fireInterval = 0.25f;        // 两发之间的间隔（秒）

    // 覆盖基类：枪械创建 GunInstance
    public override ItemInstance CreateInstance() => new GunInstance(this);

    public override ActionBase GetCurrentActions()
    {
        if (providedActions == null || providedActions.Count == 0)
        {
            return null;
        }
        ActionBase action = providedActions.FirstOrDefault();//直接返回第一个行动，我们会有更好的办法的 TODO
        action.costAP = costAP;
        action.actionDelay = fireInterval;
        return action;
    }

    public override int GetBaseDamage()
    {
        GunFireAction action = GetCurrentActions() as GunFireAction;
        if (action == null) return 0;
        return action.gunInstance.GetCurrentProjectileInfo().projectileDamage;
    }

    public override float GetArmorPenetration()
    {
        GunFireAction action = GetCurrentActions() as GunFireAction;
        if (action == null) return 0;
        return action.gunInstance.GetCurrentProjectileInfo().projectileArmorPenetration;
    }
}
public enum FireMode { SemiAuto, FullAuto, Burst }