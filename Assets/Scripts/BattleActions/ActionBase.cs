using UnityEngine;

public abstract class ActionBase : ScriptableObject, IBattleAction
{
    [SerializeField] protected string _actionName;
    [SerializeField] public int _costAP;
    //如果玩家执行这个行动，则actionDelay秒时间内不能执行任何行动，敌人也是如此
    //If the player execute this action, they will not be able to execute any actions within the actionDelay seconds, and the same applies to the enemies.
    [SerializeField] public float actionDelay;

    [SerializeField] public string actionLog;
    public string actionName
    {
        get => _actionName;
        set => _actionName = value;
    }

    public int costAP
    {
        get => _costAP;
        set => _costAP = value;
    }

    public abstract bool CanExecute(BattleEntityBase user, BattleEntityBase[] target);
    public abstract void Execute(BattleEntityBase user, BattleEntityBase[] target);

    public string ProcessBattleLog(string template, BattleEntityBase user, BattleEntityBase target, int value)
    {
        return template
            .Replace("[USER]", $"<color=green>{user.memberName}</color>")
            .Replace("[TARGET]", $"<color=red>{target.memberName}</color>")
            .Replace("[VALUE]", $"<b>{value}</b>");
    }
}
