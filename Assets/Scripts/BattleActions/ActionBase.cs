using UnityEngine;

public abstract class ActionBase : ScriptableObject, IBattleAction
{
    [SerializeField] private string _actionName;
    [SerializeField] private int _costAP;

    public string actionName { get => _actionName; }
    public int costAP { get => _costAP; }

    public abstract bool CanExecute(BattleEntityBase user, BattleEntityBase[] target);
    public abstract void Execute(BattleEntityBase user, BattleEntityBase[] target);
}
