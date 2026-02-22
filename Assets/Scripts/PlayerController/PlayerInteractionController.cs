using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private float interactRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;

    public OverworldTargetSelectorGUI targetSelectorGUI;

    public void TryInteract(IInteractable interactable)
    {
        if (interactable == null) return;
        var cols = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);
        var targetCollider = targetSelectorGUI.GetCurrentCollider();
        if (targetCollider == null)
            return;

        if (!cols.Contains(targetCollider))
            return;


        var player = partyManager.currentPlayerEntity;
        var handItem = player.GetCurrentActiveHandItem();
        if (interactable.CanInteract(handItem))
            interactable.Interact(handItem, player);
    }
}