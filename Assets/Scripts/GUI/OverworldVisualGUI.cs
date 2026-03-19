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
    [SerializeField] private OverworldTargetSelectorGUI targetSelectorGUI;
    [SerializeField] private HealthBarControllerGUI[] healthBarsGUI;
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

    public void UpdateGUI()
    {
        UpdateHandVisuals();
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
