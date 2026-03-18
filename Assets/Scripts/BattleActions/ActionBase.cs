using UnityEngine;
using UnityEngine.Localization;

public abstract class ActionBase : ScriptableObject, IBattleAction
{
    [SerializeField] protected string _actionName;
    [SerializeField] public int _costAP;
    //如果玩家执行这个行动，则actionDelay秒时间内不能执行任何行动，敌人也是如此
    //If the player execute this action, they will not be able to execute any actions within the actionDelay seconds, and the same applies to the enemies.
    [SerializeField] public float actionDelay;

    [SerializeField] public LocalizedString actionLogTemplate;
    public static System.Action<string> OnActionLogged;

    [Header("Overworld Usage")]
    // 这个 Action 是否可以在大地图使用
    public bool canUseInOverworld = false;
    public string actionName
    {
        get => _actionName;
        set => _actionName = value;
    }

    public int costAP
    {
        get => _costAP;
        set => _costAP = value;
    }
    // ── 战斗入口────────────────────────────
    public abstract bool CanExecute(BattleEntityBase user, BattleEntityBase[] target);
    public abstract void Execute(BattleEntityBase user, BattleEntityBase[] target);

    // ── 大地图入口────────────────────
    // 返回 false 表示"此 Action 不支持大地图使用"
    public virtual bool CanExecuteInOverworld(CurrentPartyMemberInfo user) => false;

    public virtual void ExecuteInOverworld(CurrentPartyMemberInfo user, InventoryManager inventory) { }

    public virtual int GetCostAP(BattleEntityBase user) => _costAP;
    public virtual float GetActionDelay(BattleEntityBase user) => actionDelay;
}
