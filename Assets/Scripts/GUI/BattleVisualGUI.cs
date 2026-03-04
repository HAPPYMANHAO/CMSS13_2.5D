using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleVisualGUI : BaseVisualGUI
{
    [SerializeField] private BattleEntityManager battleEntityManager;
    private InventoryManager inventoryManager;
    [SerializeField] private HealthBarControllerGUI[] healthBarsGUI;
    [SerializeField] private TargetSelectorGUI targetSelectorGUI;
    [SerializeField] private ItemContainerGUI containerGUI;
    [SerializeField] public Button playerEndTurnButtonGUI;

    public static Action OnPlayerEndTurn;

    InputAction activeHoldItem;//TODO
    InputAction changeActiveHand;

    // ── 实现基类的抽象方法 ──
    protected override IHandsOwner GetCurrentPlayer()
        => battleEntityManager.currentPlayerEntity;

    protected override List<ItemInstance> GetInventoryItems()
        => inventoryManager.GetAllItems();

    //---------------------BackpackGUI---------------------//
    protected override void OnOpenBackpack()
    {
        containerGUI.gameObject.SetActive(true);
        containerGUI.UpdateItemContainerGUI(GetInventoryItems());
    }

    protected override void Start()
    {
        base.Start(); // 执行基类的按钮绑定等通用初始化
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        containerGUI.battleEntityManager = battleEntityManager;
        playerEndTurnButtonGUI.onClick.AddListener(HandleEndTurnClick);
    }

    protected override void Update()
    {
        base.Update(); // 执行计时器和换手输入

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (targetSelectorGUI.GetCurrentTarget() != null && isPlayerCanExecuteAction)
                battleEntityManager.PlayerComfirmTarget(targetSelectorGUI.GetCurrentTarget());
        }
    }

    //---------------------HealthBarGUI------------------------//

    public void BindHealthBar(PartyBattleEntity entity)
    {
        for (int i = 0; i < healthBarsGUI.Length; i++)
        {
            if (healthBarsGUI[i].owner == null || string.IsNullOrEmpty(healthBarsGUI[i].owner.memberName))
            {
                healthBarsGUI[i].HealthBarBind(entity);
                break;
            }
        }
    }
    //---------------------EndTurnGUI---------------------//
    private void HandleEndTurnClick()
    {
        if (battleEntityManager.turnManager.battleState == BattleTurnManager.BattleState.PlayerTurnAction)
        {
            OnPlayerEndTurn?.Invoke();
        }
    }
}
