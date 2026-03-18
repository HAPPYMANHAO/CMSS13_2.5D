using System.Collections.Generic;
using UnityEngine;

public class OverworldActionController : MonoBehaviour
{
    private PartyManager partyManager;
    private InventoryManager inventoryManager;
    public static OverworldActionController instance;//self

    private Dictionary<string, EnemyInfo> enemyDatabase = new Dictionary<string, EnemyInfo>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        inventoryManager = InventoryManager.instance;
        partyManager = PartyManager.instance;
    }

    public void TryUseHandItem()
    {
        var player = partyManager.currentPlayerEntity;
        var handItem = player.GetCurrentActiveHandItem();
        if (handItem == null) return;
        ActionBase action = handItem.GetCurrentAction();
        // 注意：StackableItemInstance.GetCurrentAction() 会自动消耗数量
        // 但我们在 ExecuteInOverworld 里手动消耗，所以这里只取 Action 不消耗
        action = handItem.itemData is HoldableBase holdable
            ? holdable.GetCurrentActions()
            : null;

        if (action == null) return;
        if (!action.canUseInOverworld) return;
        if (!action.CanExecuteInOverworld(player)) return;

        action.ExecuteInOverworld(player, inventoryManager);
    }
}
