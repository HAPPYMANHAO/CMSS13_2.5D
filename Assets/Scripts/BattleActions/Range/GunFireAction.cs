using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Actions/GunAction")]
public class GunFireAction : ActionBase
{
    public GunInstance gunInstance;

    public override bool CanExecute(BattleEntityBase user, BattleEntityBase[] targets)
    {
        // 获取手中的枪
        if (user is not PartyBattleEntity party) return false;
        var gun = party.GetCurrentActiveHandItem() as GunInstance;
        if (gun == null || gun.IsEmpty) return false;

        bool targetAlive = targets.Length > 0 && !targets[0].EntityIsDead();
        gunInstance = party.GetCurrentActiveHandItem() as GunInstance;
        return targetAlive && user.currentAP >= costAP;
    }

    public override void Execute(BattleEntityBase user, BattleEntityBase[] targets)
    {
        if (user is not PartyBattleEntity party) return;
        gunInstance = party.GetCurrentActiveHandItem() as GunInstance;
        if (gunInstance == null) return;

        var launcher = user.battleVisual.GetComponentInChildren<ProjectileLauncher>();
        var projectileInfo = gunInstance.GetCurrentProjectileInfo();

        if (launcher != null && projectileInfo != null && targets.Length > 0)
        {
            launcher.ProjectileLaunch(user, targets[0], this,  projectileInfo);
            gunInstance.ConsumeAmmo(); // 消耗弹药
            user.battleVisual.RightLightOn();
        }
    }
}
