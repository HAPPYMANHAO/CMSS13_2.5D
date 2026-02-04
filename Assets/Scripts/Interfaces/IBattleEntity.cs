using UnityEngine;
public interface IBattleEntity 
{
        int currentAP { get; set; }

        void ExecuteAction(IBattleAction battleAction, IBattleEntity target);
        void TakeDamage(int amount, DamageType type);
}
