using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OverworldVisualGUI : BaseVisualGUI
{
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private PlayerInteractionController interactionController;
    [SerializeField] private OverworldItemContainerGUI containerGUI;
    [SerializeField] private OverworldTargetSelectorGUI targetSelectorGUI;
    [SerializeField] private HealthBarControllerGUI[] healthBarsGUI;

    private InventoryManager inventoryManager;
    protected override void OnEnable()
    {
        base.OnEnable();
        PartyManager.OnPartyMemberUpdated += UpdateGUI;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PartyManager.OnPartyMemberUpdated -= UpdateGUI;
    }

    protected override void Start()
    {
        base.Start();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        containerGUI.pratyManager = partyManager;
        interactionController.targetSelectorGUI = targetSelectorGUI;
        UpdateGUI();
    }

    protected override void Update()
    {
        base.Update();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            interactionController.TryInteract(targetSelectorGUI.GetCurrentTarget());
        }
    }


    public override IHandsOwner GetCurrentPlayer()
        => partyManager.currentPlayerEntity;

    protected override List<ItemInstance> GetInventoryItems()
        => inventoryManager.GetAllItems();

    //---------------------BackpackGUI---------------------//
    protected override void OnOpenBackpack()
    {
        containerGUI.gameObject.SetActive(true);
        containerGUI.UpdateItemContainerGUI(GetInventoryItems());
    }

    public void UpdateGUI()
    {
        UpdateHandVisuals();
        containerGUI.UpdateItemContainerGUI(GetInventoryItems());

        foreach (var healthBar in healthBarsGUI)
        {
            if (healthBar.owner != null)
            {
                healthBar.UpdateHealthBar();
            } 
        }
    }

    //---------------------HealthBarGUI------------------------//

    public void BindHealthBar(CurrentPartyMemberInfo memberInfo)
    {
        for (int i = 0; i < healthBarsGUI.Length; i++)
        {
            if (healthBarsGUI[i].owner == null)
            {
                healthBarsGUI[i].HealthBarBind(memberInfo);
                break;
            }
        }
    }
}
