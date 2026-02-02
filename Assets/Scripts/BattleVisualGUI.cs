using System.Collections.Generic;
using UnityEngine;

public class BattleVisualGUI : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Image healthBar;
    private Animator healthBarAnimator;
    private List<HealthBarEntry> healthBarDefine;

    private bool IsCritAnimationActive = false;

    private const string IS_HEALTH_CRIT_PARAM = "IsHealthCRIT";

    private void Awake()
    {
        healthBarAnimator = healthBar.GetComponent<Animator>();
    }

    public void UpdateHealth(int currentHealth)
    {
        bool healthCRIT = currentHealth < 0;

        if (healthCRIT != IsCritAnimationActive)
        {
            IsCritAnimationActive = healthCRIT;
            healthBarAnimator.SetBool(IS_HEALTH_CRIT_PARAM, IsCritAnimationActive);
        }

        if(!IsCritAnimationActive)
        {
            for (int i = 0; i < healthBarDefine.Count; i++)
            {
                if (currentHealth >= healthBarDefine[i].healthThreshold)
                {
                    healthBar.sprite = healthBarDefine[i].sprite;
                    break;
                }
            }
        }
    }
}
