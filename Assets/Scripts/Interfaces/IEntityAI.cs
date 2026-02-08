using UnityEngine;

public interface IEntityAI
{
    DecisionAI GetDecisionAI(BattleEntityBase self, BattleEntityManager entityManager);
    ActionBase ExecuteAI(BattleEntityBase self, DecisionAI decisionAI);
}
