using UnityEngine;
[CreateAssetMenu(fileName = "New Ranged Action", menuName = "Battle/Actions/Ranged Action")]
public class RangedAction : ActionBase
{
    [Header("Ranged Settings")]
    public ProjectileInfo projectileInfo;

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
        userEntity.EntityConsumeAP(costAP);

        foreach (var currentTarget in target)
        {
            ProjectileLauncher launcher = userEntity.battleVisual.GetComponentInChildren<ProjectileLauncher>();

            if (launcher != null)
            {
                //直接遍历目标，不是很好的做法，有待改进ToDo
                launcher.ProjectileLaunch(userEntity, currentTarget, this);
                ActiveActionViusal(userEntity);
                break;
            }
        }
    }

    private void ActiveActionViusal(BattleEntityBase userEntity)
    {
        userEntity.battleVisual.RightLightOn();//gun fire
    }
}
