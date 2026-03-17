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

    protected override IHandsOwner GetCurrentPlayer()
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
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PartyManager.OnPartyMemberUpdated += UpdateHandVisuals;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PartyManager.OnPartyMemberUpdated -= UpdateHandVisuals;
    }

    protected override void Start()
    {
        base.Start();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        containerGUI.pratyManager = partyManager;
        interactionController.targetSelectorGUI = targetSelectorGUI;
        UpdateHandVisuals();
    }

    protected override void Update()
    {
        base.Update();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            interactionController.TryInteract(targetSelectorGUI.GetCurrentTarget());
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

    public void BindHealthBar(CurrentPartyMemberInfo memberInfo)
    {
        for (int i = 0; i < healthBarsGUI.Length; i++)
        {
            if (healthBarsGUI[i].ownerOverworld == null || string.IsNullOrEmpty(healthBarsGUI[i].ownerOverworld.memberName))
            {
                healthBarsGUI[i].HealthBarBind(memberInfo);
                break;
            }
        }
    }
}
