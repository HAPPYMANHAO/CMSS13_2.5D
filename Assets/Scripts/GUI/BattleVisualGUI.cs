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
    [SerializeField] private HealthBarControllerGUI[] healthBars;
    [SerializeField] private TargetSelectorGUI targetSelectorGUI;
    //healthBarDefine必须按照threshold从大到小进行排序 healthBarDefine must be sorted by threshold DESC
    [SerializeField] private List<HealthBarEntry> healthBarDefine;

    [SerializeField] private HandControllerGUI rightHandButton;
    [SerializeField] private HandControllerGUI leftHandButton;

    [SerializeField] private Button backpack;
    [SerializeField] public Button playerEndTurnButton;

    public static Action OnPlayerEndTurn;

    InputAction activeHoldItem;//TODO
    InputAction changeActiveHand;

    public bool isPlayerCanExecuteAction = true;
    public float playerActionDelayTimer = 0f;

    public bool isLeftHandMain { get; private set; } = true;

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
        activeHoldItem = InputSystem.actions.FindAction(CustomInputString.ACTIVE_HOLD_ITEM);
        changeActiveHand = InputSystem.actions.FindAction(CustomInputString.CHANGE_ACTIVE_HAND);

        playerEndTurnButton.onClick.AddListener(HandleEndTurnClick);

        SetHealthBar(healthBarDefine);

        leftHandButton.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButton.handButton.onClick.AddListener(SelectRightHand);
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
        isLeftHandMain = true;
        UpdateHandVisuals();
    }

    public void SelectRightHand()
    {
        isLeftHandMain = false;
        UpdateHandVisuals();
    }

    public void ChangeActiveHand()
    {
        isLeftHandMain = !isLeftHandMain; 
        UpdateHandVisuals();
    }

    private void UpdateHandVisuals()
    {
        if (battleEntityManager?.currentPlayerEntity == null) return;

        var player = battleEntityManager.currentPlayerEntity;

        leftHandButton.baseSprite.sprite = isLeftHandMain ? leftHandButton.activeSprite : leftHandButton.disactiveSprite;
        rightHandButton.baseSprite.sprite = isLeftHandMain ? rightHandButton.disactiveSprite : rightHandButton.activeSprite;

        UpdateSingleHandVisual(leftHandButton, player.leftHandEquipment?.item);
        UpdateSingleHandVisual(rightHandButton, player.rightHandEquipment?.item);

        leftHandButton.handButton.interactable = true;
        rightHandButton.handButton.interactable = true;
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
        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].healthBarDefine = healthBarDefine;
        }
    }

    public void BindHealthBar(PartyBattleEntity entity)
    {
        for (int i = 0; i < healthBars.Length; i++)
        {
            if (healthBars[i].owner == null || string.IsNullOrEmpty(healthBars[i].owner.memberName))
            {
                healthBars[i].HealthBarBind(entity);
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
}
