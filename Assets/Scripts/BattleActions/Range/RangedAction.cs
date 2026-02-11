using UnityEngine;
[CreateAssetMenu(fileName = "New Ranged Action", menuName = "Battle/Actions/Ranged Action")]
public class RangedAction : ActionBase
{
    [Header("Ranged Settings")]
    public ProjectileInfo projectileInfo;
    
    private ProjectileLauncher launcher;

    public override bool CanExecute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        bool targetAlive = true;
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i].EntityIsDead())
            {
                targetAlive = false;
                break;
            }
        }
        return target.Length > 0 && targetAlive && userEntity.currentAP >= costAP;
    }

    public override void Execute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        if(launcher == null)
        {
            launcher = userEntity.battleVisual.GetComponentInChildren<ProjectileLauncher>();
        }

        if (target.Length > 0)
        {
            if (launcher != null)
            {
                // 单发
                launcher.ProjectileLaunch(userEntity, target[0], this);
                ActiveActionViusal(userEntity);
            }
        }
    }

    private void ActiveActionViusal(BattleEntityBase userEntity)
    {
        userEntity.battleVisual.RightLightOn();//gun fire
    }
}
