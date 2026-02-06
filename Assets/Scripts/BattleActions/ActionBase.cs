using UnityEngine;

public abstract class ActionBase : ScriptableObject, IBattleAction
{
    [SerializeField] private string _actionName;
    [SerializeField] private int _costAP;

    [SerializeField]private string acitonLog;
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
