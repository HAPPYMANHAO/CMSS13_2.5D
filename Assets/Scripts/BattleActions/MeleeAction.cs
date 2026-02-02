using UnityEngine;

public class MeleeAction : IBattleAction
{
    public string actionName { get; } = "Melee";
    public int costAP { get; } = 10;

    public bool CanExecute(IBattleEntity user, IBattleEntity[] target)
    {
        return target.Length > 0;//AP判断还没写
    }
    public void Execute(IBattleEntity user, IBattleEntity[] target)
    {
        //造成伤害
    }
}
