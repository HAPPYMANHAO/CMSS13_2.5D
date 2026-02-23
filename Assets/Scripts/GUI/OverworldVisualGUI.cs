using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OverworldVisualGUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractionController interactionController;

    [SerializeField] private PartyManager partyManager;
    private InventoryManager inventoryManager;
    [SerializeField] private HealthBarControllerGUI[] healthBarsGUI;
    [SerializeField] private OverworldItemContainerGUI containerGUI;
    [SerializeField] private Button backpackGUI;

    [SerializeField] private HandControllerGUI rightHandButtonGUI;
    [SerializeField] private HandControllerGUI leftHandButtonGUI;
    [SerializeField] OverworldTargetSelectorGUI targetSelectorGUI; 

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
        PartyManager.OnPartyMemberUpdated += UpdateHandVisuals;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        PartyManager.OnPartyMemberUpdated -= UpdateHandVisuals;
    }

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        containerGUI.pratyManager = partyManager;
        interactionController.targetSelectorGUI = targetSelectorGUI;

        activeHoldItem = InputSystem.actions.FindAction(CustomInputString.ACTIVE_HOLD_ITEM);
        changeActiveHand = InputSystem.actions.FindAction(CustomInputString.CHANGE_ACTIVE_HAND);

        leftHandButtonGUI.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButtonGUI.handButton.onClick.AddListener(SelectRightHand);
        backpackGUI.onClick.AddListener(HandleOpenBackpack);

        UpdateHandVisuals();
    }

    private void Update()
    {
        if (changeActiveHand.WasPressedThisFrame())
        {
            ChangeActiveHand();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            interactionController.TryInteract(targetSelectorGUI.GetCurrentTarget());
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
        partyManager.currentPlayerEntity.currentActiveHand = EntityHandsSlot.Left;
        UpdateHandVisuals();
    }

    public void SelectRightHand()
    {
        partyManager.currentPlayerEntity.currentActiveHand = EntityHandsSlot.Right;
        UpdateHandVisuals();
    }

    public void ChangeActiveHand()
    {
        if (partyManager.currentPlayerEntity.currentActiveHand == EntityHandsSlot.Left)
        {
            partyManager.currentPlayerEntity.currentActiveHand = EntityHandsSlot.Right;
        }
        else if (partyManager.currentPlayerEntity.currentActiveHand == EntityHandsSlot.Right)
        {
            partyManager.currentPlayerEntity.currentActiveHand = EntityHandsSlot.Left;
        }
        UpdateHandVisuals();
    }

    public void UpdateHandVisuals()
    {
        if (partyManager?.currentPlayerEntity == null) return;

        var player = partyManager.currentPlayerEntity;

        leftHandButtonGUI.baseSprite.sprite = player.currentActiveHand == EntityHandsSlot.Left ? leftHandButtonGUI.activeSprite : leftHandButtonGUI.disactiveSprite;
        rightHandButtonGUI.baseSprite.sprite = player.currentActiveHand == EntityHandsSlot.Left ? rightHandButtonGUI.disactiveSprite : rightHandButtonGUI.activeSprite;

        UpdateSingleHandVisual(leftHandButtonGUI, player.leftHandEquipment?.item);
        UpdateSingleHandVisual(rightHandButtonGUI, player.rightHandEquipment?.item);

        leftHandButtonGUI.handButton.interactable = true;
        rightHandButtonGUI.handButton.interactable = true;
    }

    private void UpdateSingleHandVisual(HandControllerGUI handGUI, ItemInstance item)
    {
        if (item != null)
        {
            handGUI.EnableHoldItemSprite();
            handGUI.holdItemImage.sprite = item.itemData.icon;
            handGUI.UpdateQuantityDisplay(item);
        }
        else
        {
            handGUI.holdItemImage.sprite = null;
            handGUI.DisableHoldItemSprite();
            handGUI.UpdateQuantityDisplay(item);
        }
    }

    public void HandleHandStatsChanged()
    {
        UpdateHandVisuals();
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
    //---------------------BackpackGUI---------------------//
    private void HandleOpenBackpack()
    {
        containerGUI.gameObject.SetActive(true);
        containerGUI.UpdateItemContainerGUI(inventoryManager.GetAllItems());
    }
}
