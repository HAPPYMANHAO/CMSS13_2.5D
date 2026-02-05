using UnityEngine;

public class MeleeAction : IBattleAction
{
    public string actionName { get; } = "Melee";
    public int costAP { get; } = 15;

    public bool CanExecute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        return target.Length > 0 && userEntity.currentAP >= costAP;
    }
    public void Execute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        //造成伤害
    }
}
