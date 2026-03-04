using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class BaseVisualGUI : MonoBehaviour
{

    protected abstract IHandsOwner GetCurrentPlayer();
    protected abstract List<ItemInstance> GetInventoryItems();
    protected abstract void OnOpenBackpack();   


    [SerializeField] protected HandControllerGUI rightHandButtonGUI;
    [SerializeField] protected HandControllerGUI leftHandButtonGUI;
    [SerializeField] protected Button backpackGUI;

    public bool isPlayerCanExecuteAction = true;
    public float playerActionDelayTimer = 0f;

    protected PlayerControls inputActions;
    private InputAction changeActiveHand;

    protected virtual void Awake()
    {
        inputActions = new PlayerControls();
    }

    protected virtual void OnEnable() { inputActions.Enable(); }
    protected virtual void OnDisable() { inputActions.Disable(); }

    protected virtual void Start()
    {
        changeActiveHand = InputSystem.actions.FindAction(CustomInputString.CHANGE_ACTIVE_HAND);
        leftHandButtonGUI.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButtonGUI.handButton.onClick.AddListener(SelectRightHand);
        backpackGUI.onClick.AddListener(OnOpenBackpack);
    }

    protected virtual void Update()
    {
        if (changeActiveHand.WasPressedThisFrame())
            ChangeActiveHand();

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

    //--------------------Hand GUI----------------------//

    public void SelectLeftHand()
    {
        GetCurrentPlayer().currentActiveHand = EntityHandsSlot.Left;
        UpdateHandVisuals();
    }

    public void SelectRightHand()
    {
        GetCurrentPlayer().currentActiveHand = EntityHandsSlot.Right;
        UpdateHandVisuals();
    }

    public void ChangeActiveHand()
    {
        var player = GetCurrentPlayer();
        player.currentActiveHand = player.currentActiveHand == EntityHandsSlot.Left
            ? EntityHandsSlot.Right : EntityHandsSlot.Left;
        UpdateHandVisuals();
    }

    public void UpdateHandVisuals()
    {
        var player = GetCurrentPlayer();
        if (player == null) return;

        leftHandButtonGUI.baseSprite.sprite = player.currentActiveHand == EntityHandsSlot.Left
            ? leftHandButtonGUI.activeSprite : leftHandButtonGUI.disactiveSprite;
        rightHandButtonGUI.baseSprite.sprite = player.currentActiveHand == EntityHandsSlot.Left
            ? rightHandButtonGUI.disactiveSprite : rightHandButtonGUI.activeSprite;

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
        }
        else
        {
            handGUI.holdItemImage.sprite = null;
            handGUI.DisableHoldItemSprite();
        }
        handGUI.UpdateQuantityDisplay(item);
    }
}