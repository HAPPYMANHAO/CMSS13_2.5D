using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(menuName = "Battle/MeleeAction")]
public class MeleeAction : ActionBase
{
    [Header("Action Settings")]
    public DamageType damageType;
    public int baseDamage;

    public override bool CanExecute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        return target.Length > 0 && userEntity.currentAP >= costAP;
    }
    public override void Execute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].EntityTakeDamage(baseDamage, damageType);
        }
    }
}
