using UnityEngine;
public interface IBattleEntity
{
    string memberName { get; }
    int currentHealth { get; set; }
    int maxHealth { get; }
    int currentAP { get; set; }
    int maxAP { get; }
    bool isPlayerFaction { get; }

    void ExecuteAction(ActionBase battleAction, BattleEntityBase target);
    void EntityTakeDamage(int amount, DamageType type);
}
