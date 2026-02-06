using UnityEngine;

public interface IBattleAction
{
    string actionName { get; set; }
    int costAP { get; set; }

    bool CanExecute(BattleEntityBase user, BattleEntityBase[] target);
    void Execute(BattleEntityBase user, BattleEntityBase[] target);
}
