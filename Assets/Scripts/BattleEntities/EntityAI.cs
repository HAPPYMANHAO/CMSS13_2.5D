using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAI", menuName = "Battle/AI/RandomAI")]
public class EntityAI : ScriptableObject, IEntityAI
{
    [SerializeField] public List<AIActionWeight> actionPool;

    public DecisionAI GetDecisionAI(BattleEntityBase self, BattleEntityManager entityManager)
    {
        BattleEntityBase[] targets = SetRandomTarget(self, entityManager);
        ActionBase currentAction = ComfirmAction();

        targets = targets.Where(entity => !entity.EntityIsDead()).ToArray();

        if (targets == null || targets.Length == 0 || currentAction == null)
        {
            return new DecisionAI { action = null, targets = null };
        }
        return new DecisionAI { action = currentAction, targets = targets };
    }


    public ActionBase ExecuteAI(BattleEntityBase self, DecisionAI decisionAI)
    {       
        if (decisionAI.targets.Length > 0 && decisionAI.targets != null && actionPool.Count > 0 && decisionAI.action != null)
        {
            self.ExecuteAction(decisionAI.action, new BattleEntityBase[] { decisionAI.targets[0] });
            //目前实际上只选择一个目标进行行动，AI暂时不支持多目标(实际上玩家GUI目前也不行)
        }

        return decisionAI.action;
    }

    public ActionBase ComfirmAction()
    {
        //Random
        return actionPool[Random.Range(0, actionPool.Count)].action;
    }

    private BattleEntityBase[] SetRandomTarget(BattleEntityBase self, BattleEntityManager entityManager)
    {
        var allTargets = entityManager.allBattleEntities
            .Where(entity => entity != null &&
                             entity.entityFaction != self.entityFaction &&
                             !entity.EntityIsDead())
            .ToList();

        if (allTargets.Count == 0) return null;

        int randomIndex = Random.Range(0, allTargets.Count);
        return new BattleEntityBase[] { allTargets[randomIndex] };
    }
}
