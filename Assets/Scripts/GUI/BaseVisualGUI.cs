using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class BaseVisualGUI : MonoBehaviour
{

    public abstract IHandsOwner GetCurrentPlayer();
    protected abstract List<ItemInstance> GetInventoryItems();
    protected abstract void OnOpenBackpack();   

    [SerializeField] protected HandControllerGUI rightHandButtonGUI;
    [SerializeField] protected HandControllerGUI leftHandButtonGUI;
    [SerializeField] protected Button backpackGUI;

    public bool isPlayerCanExecuteAction = true;
    public float playerActionDelayTimer = 0f;

    protected PlayerControls inputActions;
    protected InputAction changeActiveHand;
    protected InputAction activeHoldItem;

    protected virtual void Awake()
    {
        inputActions = new PlayerControls();
    }

    protected virtual void OnEnable() { inputActions.Enable(); }
    protected virtual void OnDisable() { inputActions.Disable(); }

    protected virtual void Start()
    {
        changeActiveHand = InputSystem.actions.FindAction(CustomInputString.CHANGE_ACTIVE_HAND);
        activeHoldItem = InputSystem.actions.FindAction(CustomInputString.ACTIVE_HOLD_ITEM);
        leftHandButtonGUI.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButtonGUI.handButton.onClick.AddListener(SelectRightHand);
        backpackGUI.onClick.AddListener(OnOpenBackpack);  
    }

    protected virtual void Update()
    {
        if (changeActiveHand.WasPressedThisFrame())
            ChangeActiveHand();
        if (activeHoldItem.WasPerformedThisFrame())
            ToggleBothHandsUse();

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
        if (IsBothHandsUsing())
        {
            return;
        }
        GetCurrentPlayer().currentActiveHand = EntityHandsSlot.Left;
        UpdateHandVisuals();
    }

    public void SelectRightHand()
    {
        if (IsBothHandsUsing())
        {
            return;
        }
        GetCurrentPlayer().currentActiveHand = EntityHandsSlot.Right;
        UpdateHandVisuals();
    }

    public void ChangeActiveHand()
    {
        if (IsBothHandsUsing())
        {
            return;
        }
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

        if(player.GetCurrentActiveHandItem() != null)
        {
            if (player.GetCurrentActiveHandItem().isBothHandsUsing)
            {
                if (player.currentActiveHand == EntityHandsSlot.Right)
                {
                    leftHandButtonGUI.EnableHandOccupiedImage();
                }
                else if (player.currentActiveHand == EntityHandsSlot.Left)
                {
                    rightHandButtonGUI.EnableHandOccupiedImage();
                }
            }
            else
            {
                leftHandButtonGUI.DisableHandOccupiedImage();
                rightHandButtonGUI.DisableHandOccupiedImage();
            }
        }

        UpdateSingleHandVisual(leftHandButtonGUI, player.leftHandEquipment?.item);
        UpdateSingleHandVisual(rightHandButtonGUI, player.rightHandEquipment?.item);

        bool isBoth = IsBothHandsUsing();

        if (player.currentActiveHand == EntityHandsSlot.Right)
        {
            leftHandButtonGUI.handButton.interactable = !isBoth;
        }
        else if (player.currentActiveHand == EntityHandsSlot.Left)
        {
            rightHandButtonGUI.handButton.interactable = !isBoth;
        }       
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
    public virtual void ToggleBothHandsUse()
    {
        IHandsOwner player = GetCurrentPlayer();

        var item = player.GetCurrentActiveHandItem();
        if (item == null || item.itemData is not WeaponBase weapon) return;

        if (item.isBothHandsUsing)
        {
            item.OnExitBothHandUse();
        }
        else
        {   
            if(player is PartyBattleEntity)
            {
                var entity = player as PartyBattleEntity;
                if (CheckAnotherHandOccupied(player))
                {
                    return;
                }

                if (!weapon.CanBothHandUse(entity)) return;
                item.OnBothHandUse();
                entity.EntityConsumeAP(weapon.enterBothHandsUseCostAP);
            }
            else if(player is CurrentPartyMemberInfo)
            {
                if (CheckAnotherHandOccupied(player))
                {
                    return;
                }
                if (!weapon.allowUseOfBothHands) return;
                item.OnBothHandUse();
            }
            
        }
        UpdateHandVisuals();
    }

    private bool CheckAnotherHandOccupied(IHandsOwner entity)
    {
        if (entity.currentActiveHand == EntityHandsSlot.Right && !entity.leftHandEquipment.IsEmpty)
        {
            return true;
        }
        else if (entity.currentActiveHand == EntityHandsSlot.Left && !entity.rightHandEquipment.IsEmpty)
        {
            return true;
        }
        return false;
    }
    private bool IsBothHandsUsing()
    {
        var player = GetCurrentPlayer();
        var item = player?.GetCurrentActiveHandItem();
        return item != null && item.isBothHandsUsing;
    }
}

