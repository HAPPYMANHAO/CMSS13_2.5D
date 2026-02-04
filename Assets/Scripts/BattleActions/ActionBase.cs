using UnityEngine;

public abstract class ActionBase : ScriptableObject, IBattleAction
{
    [SerializeField] private string _actionName;
    [SerializeField] private int _costAP;

    public string actionName { get; }
    public int costAP { get; }

    public abstract bool CanExecute(IBattleEntity user, IBattleEntity[] target);
    public abstract void Execute(IBattleEntity user, IBattleEntity[] target);
}
