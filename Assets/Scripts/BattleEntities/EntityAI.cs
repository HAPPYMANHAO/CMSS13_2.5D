using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAI", menuName = "Battle/AI/RandomAI")]
public class EntityAI : ScriptableObject, IEntityAI
{
    [SerializeField] public List<AIActionWeight> actionPool;

    // ── 执行时选目标：action已确定，临时选一个当前存活的目标 ──
    public BattleEntityBase[] ResolveTarget(BattleEntityBase self, BattleEntityManager entityManager)
    {
        return SetRandomTarget(self, entityManager); 
    }

    public List<ActionBase> BuildActionQueue(BattleEntityBase self, int simulatedAP)
    {
        var queue = new List<ActionBase>();

        while (simulatedAP > 0)
        {
            ActionBase action = ComfirmAction();
            if (action == null) break;

            // GetCostAP 会读取实体身上的Buff修改（击倒Buff此时已生效）
            int cost = action.GetCostAP(self);
            if (simulatedAP < cost) break;

            queue.Add(action);
            simulatedAP -= cost;
        }

        return queue;
    }

    // 原有的无参版本直接调用新版本
    public List<ActionBase> BuildActionQueue(BattleEntityBase self)
        => BuildActionQueue(self, self.currentAP);

    public DecisionAI GetDecisionAI(BattleEntityBase self, BattleEntityManager entityManager)
    {
        BattleEntityBase[] targets = SetRandomTarget(self, entityManager);
        ActionBase currentAction = ComfirmAction();

        if (targets == null || targets.Length == 0)
            return new DecisionAI { action = null, targets = null };
        targets = targets.Where(entity => !entity.EntityIsDead()).ToArray();

        if (targets == null || targets.Length == 0 || currentAction == null)
        {
            return new DecisionAI { action = null, targets = null };
        }
        return new DecisionAI { action = currentAction, targets = targets };
    }

    // ExecuteAI目前没有被使用------------------
    public ActionBase ExecuteAI(BattleEntityBase self, DecisionAI decisionAI)
    {       
        if (decisionAI.targets != null && decisionAI.targets.Length > 0 && actionPool.Count > 0 && decisionAI.action != null)
        {
            self.ExecuteAction(decisionAI.action, new BattleEntityBase[] { decisionAI.targets[0] });
            //目前实际上只选择一个目标进行行动，AI暂时不支持多目标(实际上玩家GUI目前也不行)
        }

        return decisionAI.action;
    }
    // ExecuteAI目前没有被使用------------------

    public ActionBase ComfirmAction()
    {    
        if (!actionPool.Any()) return null;
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
