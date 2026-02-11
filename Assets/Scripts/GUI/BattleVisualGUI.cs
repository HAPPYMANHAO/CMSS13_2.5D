using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;
using static UnityEngine.EventSystems.EventTrigger;

public class BattleVisualGUI : MonoBehaviour
{
    [SerializeField] private BattleEntityManager battleEntityManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private HealthBarControllerGUI[] healthBarsGUI;
    [SerializeField] private TargetSelectorGUI targetSelectorGUI;
    [SerializeField] private ItemContainerGUI containerGUI;

    [SerializeField] private HandControllerGUI rightHandButtonGUI;
    [SerializeField] private HandControllerGUI leftHandButtonGUI;

    [SerializeField] private Button backpackGUI;
    [SerializeField] public Button playerEndTurnButtonGUI;
    //healthBarDefine必须按照threshold从大到小进行排序 healthBarDefine must be sorted by threshold DESC
    [SerializeField] private List<HealthBarEntry> healthBarDefine;

    

    public static Action OnPlayerEndTurn;

    InputAction activeHoldItem;//TODO
    InputAction changeActiveHand;

    public bool isPlayerCanExecuteAction = true;
    public float playerActionDelayTimer = 0f;   

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        containerGUI.battleEntityManager = battleEntityManager;

        activeHoldItem = InputSystem.actions.FindAction(CustomInputString.ACTIVE_HOLD_ITEM);
        changeActiveHand = InputSystem.actions.FindAction(CustomInputString.CHANGE_ACTIVE_HAND);

        playerEndTurnButtonGUI.onClick.AddListener(HandleEndTurnClick);

        SetHealthBar(healthBarDefine);

        leftHandButtonGUI.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButtonGUI.handButton.onClick.AddListener(SelectRightHand);
        backpackGUI.onClick.AddListener(HandleOpenBackpack);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (targetSelectorGUI.GetCurrentTarget() != null && isPlayerCanExecuteAction)
            {
                battleEntityManager.PlayerComfirmTarget(targetSelectorGUI.GetCurrentTarget());
            }
        }

        if (changeActiveHand.WasPressedThisFrame())
        {
            ChangeActiveHand();
        }

        if (!isPlayerCanExecuteAction)
        {
            playerActionDelayTimer -= Time.deltaTime;
            if (playerActionDelayTimer < 0)
            {
                isPlayerCanExecuteAction = true;
                playerActionDelayTimer = 0;
            }
        }
    }

    //-----------------------HandsGUI------------------------//
    public void SelectLeftHand()
    {
        battleEntityManager.currentPlayerEntity.currentActiveHand = PartyBattleEntity.EntityHandsSlot.Left;
        UpdateHandVisuals();
    }

    public void SelectRightHand()
    {
        battleEntityManager.currentPlayerEntity.currentActiveHand = PartyBattleEntity.EntityHandsSlot.Right;
        UpdateHandVisuals();
    }

    public void ChangeActiveHand()
    {
        if(battleEntityManager.currentPlayerEntity.currentActiveHand == PartyBattleEntity.EntityHandsSlot.Left)
        {
            battleEntityManager.currentPlayerEntity.currentActiveHand = PartyBattleEntity.EntityHandsSlot.Right;
        }
        else if (battleEntityManager.currentPlayerEntity.currentActiveHand == PartyBattleEntity.EntityHandsSlot.Right)
        {
            battleEntityManager.currentPlayerEntity.currentActiveHand = PartyBattleEntity.EntityHandsSlot.Left;
        }
        UpdateHandVisuals();
    }

    public void UpdateHandVisuals()
    {
        if (battleEntityManager?.currentPlayerEntity == null) return;

        var player = battleEntityManager.currentPlayerEntity;

        leftHandButtonGUI.baseSprite.sprite = player.currentActiveHand == PartyBattleEntity.EntityHandsSlot.Left ? leftHandButtonGUI.activeSprite : leftHandButtonGUI.disactiveSprite;
        rightHandButtonGUI.baseSprite.sprite = player.currentActiveHand == PartyBattleEntity.EntityHandsSlot.Left ? rightHandButtonGUI.disactiveSprite : rightHandButtonGUI.activeSprite;

        UpdateSingleHandVisual(leftHandButtonGUI, player.leftHandEquipment?.item);
        UpdateSingleHandVisual(rightHandButtonGUI, player.rightHandEquipment?.item);

        leftHandButtonGUI.handButton.interactable = true;
        rightHandButtonGUI.handButton.interactable = true;
    }

    private void UpdateSingleHandVisual(HandControllerGUI handGUI, HoldableBase item)
    {
        if (item != null && !string.IsNullOrEmpty(item.GetCurrentActions().actionName))
        {
            handGUI.EnableHoldItemSprite();
            handGUI.holdItemImage.sprite = item.icon;
        }
        else
        {
            handGUI.holdItemImage.sprite = null;
            handGUI.DisableHoldItemSprite();
        }
    }

    //---------------------HealthBarGUI------------------------//
    private void SetHealthBar(List<HealthBarEntry> healthBarDefine)
    {
        for (int i = 0; i < healthBarsGUI.Length; i++)
        {
            healthBarsGUI[i].healthBarDefine = healthBarDefine;
        }
    }

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
            OnPlayerEndTurn.Invoke();
        }
    }
    //---------------------BackpackGUI---------------------//
    private void HandleOpenBackpack()
    {
        containerGUI.gameObject.SetActive(true);
        containerGUI.UpdateItemContainerGUI(inventoryManager.GetCurrentInventoryItems());
    }
}
