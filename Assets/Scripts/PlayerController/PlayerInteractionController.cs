using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour, IInteractable
{
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private float interactRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private float TryInteractDelayTimer;
    private const float TRY_INTERACT_DELAY = 0.5f;

    public OverworldTargetSelectorGUI targetSelectorGUI;
    private OverworldActionController overworldActionController;

    private void Start()
    {
        overworldActionController = OverworldActionController.instance;
    }
    private void Update()
    {
        if(TryInteractDelayTimer > 0)
        {
            TryInteractDelayTimer -= Time.deltaTime;
        }
    }

    public void TryInteract(IInteractable interactable)
    {
        if(TryInteractDelayTimer <= 0)
        {
            var player = partyManager.currentPlayerEntity;
            var handItem = player.GetCurrentActiveHandItem();
            if (interactable is PlayerInteractionController)
            {
                overworldActionController.TryUseHandItem();
            }
            else
            {
                if (interactable == null) return;
                var cols = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);
                var targetCollider = targetSelectorGUI.GetCurrentCollider();
                if (targetCollider == null)
                    return;

                if (!cols.Contains(targetCollider))
                    return;

                interactable.Interact(handItem, player);
            }
            TryInteractDelayTimer = TRY_INTERACT_DELAY;
        }         
    }

    public bool CanInteract(ItemInstance handItem)
    {
        return handItem.GetCurrentAction().canUseInOverworld;
    }

    public void Interact(ItemInstance handItem, CurrentPartyMemberInfo interactor)
    {
        overworldActionController.TryUseHandItem();
    }
}