using UnityEngine;

public interface IBattleAction
{
    string actionName { get; }
    int costAP { get; }

    bool CanExecute(IBattleEntity user, IBattleEntity[] target);
    void Execute(IBattleEntity user, IBattleEntity[] target);
}
