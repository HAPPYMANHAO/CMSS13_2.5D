using UnityEngine;

public interface IBattleEntity 
{
        int CurrentAP { get; set; }

        void ExecuteAction(int actionIndex, IBattleEntity target);
        void TakeDamage(int amount, ArmorType type);
}
