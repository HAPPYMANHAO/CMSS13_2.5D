using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleVisualGUI : MonoBehaviour
{
    [SerializeField] private HealthBarControllerGUI[] healthBars;
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
        SelectLeftHand();

        leftHandButton.handButton.onClick.AddListener(SelectLeftHand);
        rightHandButton.handButton.onClick.AddListener(SelectRightHand);
    }

    public void SelectLeftHand()
    {
        isLeftHandMain = true;
        UpdateVisuals();
    }

    public void SelectRightHand()
    {
        isLeftHandMain = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        leftHandButton.baseSprite.sprite = isLeftHandMain ? leftHandButton.activeSprite : leftHandButton.disactiveSprite;
        rightHandButton.baseSprite.sprite = isLeftHandMain ? rightHandButton.disactiveSprite : rightHandButton.activeSprite;

        leftHandButton.handButton.interactable = true;
        rightHandButton.handButton.interactable = true;
    }

    public void SetHealthBarValue(int currentHealth, int maxHealth, int shockHealth)
    {
        healthBars[1].UpdateHealth(currentHealth, maxHealth, shockHealth, healthBarDefine);
    }
}
