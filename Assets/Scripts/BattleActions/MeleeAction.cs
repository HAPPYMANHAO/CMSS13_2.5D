using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(menuName = "Battle/MeleeAction")]
public class MeleeAction : ActionBase
{
    [Header("Action Settings")]
    public DamageType damageType;
    public int baseDamage;

    public float armorPenetration = 0f;

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
            int damage = target[i].EntityTakeDamage(baseDamage, damageType, armorPenetration);

            BattleEntityBase currentTarget = target[i];

            var handleLog = actionLogTemplate.GetLocalizedStringAsync(new
            {
                user = userEntity.memberName,
                target = target[i].memberName,
                damage = damage.ToString()
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
}
