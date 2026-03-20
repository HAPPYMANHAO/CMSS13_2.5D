using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Actions/GunAction")]
public class GunFireAction : ActionBase
{
    // 射击事件 - 用于触发相机震动等效果
    public static event Action OnGunFired;

    public override bool CanExecute(BattleEntityBase user, BattleEntityBase[] targets)
    {
        if (user is not PartyBattleEntity party) return false;
        var gun = party.GetCurrentActiveHandItem() as GunInstance; 
        if (gun == null || gun.IsEmpty) return false;
        return targets.Length > 0 && !targets[0].EntityIsDead() && user.currentAP >= GetCostAP(user);
    }

    public override void Execute(BattleEntityBase user, BattleEntityBase[] targets)
    {
        if (user is not PartyBattleEntity party) return;
        var gun = party.GetCurrentActiveHandItem() as GunInstance; 
        if (gun == null) return;
        float projectileAccuracy = gun.GetAccuracy((PartyBattleEntity)user);
        var launcher = user.battleVisual.GetComponentInChildren<ProjectileLauncher>();
        var projectileInfo = gun.GetCurrentProjectileInfo();
        if (launcher != null && projectileInfo != null && targets.Length > 0)
        {
            launcher.ProjectileLaunch(user, targets[0], this, projectileInfo, projectileAccuracy);
            gun.ConsumeAmmo();
            user.battleVisual.RightLightOn();
            gun.AddRecoil((PartyBattleEntity)user);
            
            // 触发射击事件
            OnGunFired?.Invoke();
        }
    }

    public override int GetCostAP(BattleEntityBase user)
    {
        if (user is PartyBattleEntity party && party.GetCurrentActiveHandItem() is GunInstance gun)
        {
            return gun.GunData.apPerShot;
        }
        return base.GetCostAP(user); 
    }

    public override float GetActionDelay(BattleEntityBase user)
    {
        if (user is PartyBattleEntity party && party.GetCurrentActiveHandItem() is GunInstance gun)
        {
            return gun.GunData.fireInterval;
        }
        return base.GetActionDelay(user);
    }
}
