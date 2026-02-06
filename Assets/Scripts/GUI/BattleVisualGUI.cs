using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class BattleVisualGUI : MonoBehaviour
{
    [SerializeField] private BattleInfoManager battleInfoManager;
    [SerializeField] private HealthBarControllerGUI[] healthBars;
    [SerializeField] private TargetSelectorGUI targetSelectorGUI;
    //healthBarDefine必须按照threshold从大到小进行排序 healthBarDefine must be sorted by threshold DESC
    [SerializeField] private List<HealthBarEntry> healthBarDefine;

    [SerializeField] private HandControllerGUI rightHandButton;
    [SerializeField] private HandControllerGUI leftHandButton;
    [SerializeField] private Button Backpack;

    public bool isLeftHandMain { get; private set; } = true;
    private void Awake()
    {

    }

    private void Start()
    {
        SetHealthBar(healthBarDefine);

        SelectLeftHand();

        leftHandButton.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButton.handButton.onClick.AddListener(SelectRightHand);
    }

    private void Update()
    {
        if (targetSelectorGUI.GetCurrentTarget() != null)
        {
            battleInfoManager.PlayerComfirmTarget(targetSelectorGUI.GetCurrentTarget());
            Debug.Log(targetSelectorGUI.GetCurrentTarget().memberName);
        }
        else
        {
            Debug.Log("没有目标");
        }
    }

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

    private void UpdateHandVisuals()
    {
        leftHandButton.baseSprite.sprite = isLeftHandMain ? leftHandButton.activeSprite : leftHandButton.disactiveSprite;
        rightHandButton.baseSprite.sprite = isLeftHandMain ? rightHandButton.disactiveSprite : rightHandButton.activeSprite;

        leftHandButton.handButton.interactable = true;
        rightHandButton.handButton.interactable = true;
    }

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
}
