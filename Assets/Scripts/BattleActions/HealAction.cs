using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Battle/Actions/HealAction")]
public class HealAction : ActionBase
{
    [SerializeField] private int healAmount = 30;

    public override bool CanExecute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        if (userEntity.currentAP < costAP) return false;

        foreach (var t in target)
        {
            if (t.entityFaction != userEntity.entityFaction) return false;
            if (t.EntityIsDead()) return false;
        }
        return true;
    }

    public override void Execute(BattleEntityBase userEntity, BattleEntityBase[] target)
    {
        foreach (var t in target)
        {
            t.EntityRevoverHealth(healAmount);
            UpdateActionLog(userEntity, t, healAmount);
        }
    }

    public override bool CanExecuteInOverworld(CurrentPartyMemberInfo user)
    {
        // 没满血才能用
        return (user.currentHealth < user.maxHealth) && !user.isDead;
    }

    public override void ExecuteInOverworld(CurrentPartyMemberInfo user,
                                            InventoryManager inventory)
    {
        user.RevoverHealth(healAmount);

        var handItem = user.GetCurrentActiveHandItem() as StackableItemInstance;
        if (handItem != null)
        {
            if (handItem.IsEmpty)
                user.SentHoldItemToInventory();
        }

        PartyManager.PartyMemberUpdated();
    }

    public void UpdateActionLog(BattleEntityBase userEntity, BattleEntityBase target, int amount)
    {
        var handleLog = actionLogTemplate.GetLocalizedStringAsync(new
        {
            user = userEntity.memberName,
            target = target.memberName,
            healAmount = amount.ToString()
        });
        handleLog.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                string finalLog = op.Result;
                OnActionLogged?.Invoke(finalLog);
            }
        };
    }
}
