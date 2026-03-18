using System;
using System.Collections;
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
    [SerializeField] private AccuracyDisplayGUI accuracyDisplay;
    [SerializeField] protected EnemyActionQueueGUI enemyActionQueueGUI;

    public bool isPlayerTurn = false;

    public static Action OnPlayerEndTurn;

    private Coroutine _autoFireCoroutine;

    // ── 实现基类的抽象方法 ──
    public override IHandsOwner GetCurrentPlayer()
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
        UpdateHandVisuals();
    }

    protected override void OnEnable()
    {
        if (battleEntityManager != null)
            BattleEntityManager.OnPartyEntitiesSpawned += UpdateHandVisuals;
    }

    protected override void OnDisable()
    {
        if (battleEntityManager != null)
            BattleEntityManager.OnPartyEntitiesSpawned -= UpdateHandVisuals;
    }

    protected override void Update()
    {
        base.Update();

        var gun = GetGunInHand();
        if (_autoFireCoroutine != null)
        {
            if (gun == null)
            {
                StopCoroutine(_autoFireCoroutine);
                _autoFireCoroutine = null;
            }
        }

        // 左键逻辑根据射击模式分叉
        if (gun != null)
        {

            float accuracy = gun.GetAccuracy(battleEntityManager.currentPlayerEntity);
            accuracyDisplay.UpdateAccuracy(accuracy);
            accuracyDisplay.transform.position = Mouse.current.position.ReadValue();
            accuracyDisplay.Show();

            switch (gun.currentFireMode)
            {
                case FireMode.SemiAuto:
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                        TryExecuteAction();
                    break;

                case FireMode.FullAuto:
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                        _autoFireCoroutine = StartCoroutine(AutoFireRoutine(gun));
                    if (Mouse.current.leftButton.wasReleasedThisFrame && _autoFireCoroutine != null)
                    {
                        StopCoroutine(_autoFireCoroutine);
                        _autoFireCoroutine = null;
                    }
                    break;

                case FireMode.Burst:
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                        StartCoroutine(BurstFireRoutine(gun));
                    break;
            }
        }
        else
        {
            // 没有枪，走原来的点击逻辑
            if (Mouse.current.leftButton.wasPressedThisFrame)
                TryExecuteAction();

            accuracyDisplay.Hide();
        }
    }

    private void TryExecuteAction()
    {
        var target = targetSelectorGUI.GetCurrentTarget();
        if (target != null && IsPlayerCanDoAct())
        {
            var gun = GetGunInHand();
            bool isActionExecuted = battleEntityManager.PlayerComfirmTarget(target);
            if (gun != null && isActionExecuted)
            {
                float accuracy = gun.GetAccuracy(battleEntityManager.currentPlayerEntity);
                accuracyDisplay.PlayFireFeedback(accuracy);
            }
        }      
    }

    private IEnumerator AutoFireRoutine(GunInstance gun)
    {
        while (!gun.IsEmpty && IsPlayerCanDoAct())
        {
            TryExecuteAction();
            yield return new WaitForSeconds(gun.GunData.fireInterval);
        }
        _autoFireCoroutine = null;
    }

    private IEnumerator BurstFireRoutine(GunInstance gun)
    {
        int shots = gun.GunData.burstCount;
        for (int i = 0; i < shots && !gun.IsEmpty; i++)
        {
            if (!IsPlayerCanDoAct()) yield break;
            TryExecuteAction();
            yield return new WaitForSeconds(gun.GunData.fireInterval);
        }
    }

    private GunInstance GetGunInHand()
        => battleEntityManager.currentPlayerEntity?.GetCurrentActiveHandItem() as GunInstance;

    private bool IsPlayerCanDoAct()
    {
        return isPlayerTurn && isPlayerCanExecuteAction;
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
