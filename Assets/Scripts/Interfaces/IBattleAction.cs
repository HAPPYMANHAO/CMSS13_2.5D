using UnityEngine;

public interface IBattleAction
{
    string actionName { get; }
    int costAP { get; }

    bool CanExecute(BattleEntityBase user, BattleEntityBase[] target);
    void Execute(BattleEntityBase user, BattleEntityBase[] target);
}
