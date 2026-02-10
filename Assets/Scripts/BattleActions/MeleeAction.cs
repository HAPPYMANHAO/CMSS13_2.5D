using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(menuName = "Battle/MeleeAction")]
public class MeleeAction : ActionBase
{
    [Header("Action Settings")]
    public DamageType damageType;

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
        for (int i = 0; i < target.Length; i++)
        {
            int damage = target[i].EntityTakeDamage(userEntity ,damageType, this);

            BattleEntityBase currentTarget = target[i];

            UpdateActionLog(userEntity, currentTarget, damage);
        }
    }

    public void UpdateActionLog(BattleEntityBase userEntity, BattleEntityBase target, int amount)
    {
        var handleLog = actionLogTemplate.GetLocalizedStringAsync(new
        {
            user = userEntity.memberName,
            target = target.memberName,
            damage = amount.ToString()
        });
        handleLog.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                string finalLog = op.Result;
                OnActionLogged?.Invoke(finalLog);
            }
        };
    }
}
