using UnityEngine;
public interface IBattleEntity
{
    string memberName { get; }
    int currentHealth { get; set; }
    int maxHealth { get; set; }
    int healthDead { get; set; }
    int currentAP { get; set; }
    int maxAP { get; set; }
    Faction entityFaction { get; set; }

    void ExecuteAction(ActionBase battleAction, BattleEntityBase[] target);
    int EntityTakeDamage(BattleEntityBase user, DamageType damageType, ActionBase sourceAction = null);
}
