using UnityEngine;
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
            target[i].EntityTakeDamage(baseDamage, damageType, armorPenetration);
        }
    }
}
