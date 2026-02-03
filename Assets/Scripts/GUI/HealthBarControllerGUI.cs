using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBarControllerGUI : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Image overlayCRIT;
    private Animator animatorCRIT;
    private UnityEngine.UI.Image healthBarImage;
    private TextMeshProUGUI healthValue;

    private IBattleEntity owner;

    private bool IsCritAnimationActive = false;

    private void Awake()
    {
        healthBarImage = this.GetComponent<UnityEngine.UI.Image>();
        healthValue = this.transform.Find("PlayerHealthValue").GetComponent<TextMeshProUGUI>();
        animatorCRIT = overlayCRIT.GetComponent<Animator>();
    }

    public void HealthBarBind(IBattleEntity entity)
    {
        this.owner = entity;
    }

    public void UpdateHealth(int currentHealth, int maxHealth, int shockHealth, List<HealthBarEntry> healthBarDefine)
    {
        bool healthCRIT = currentHealth < 0;
        bool healthShock = currentHealth < shockHealth;
        healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();

        if (healthCRIT != IsCritAnimationActive)
        {
            IsCritAnimationActive = healthCRIT;
            overlayCRIT.gameObject.SetActive(true);
        }

        if (IsCritAnimationActive)
        {
            animatorCRIT.speed = healthShock ? 2.0f : 1.0f;
        }
        else
        {
            animatorCRIT.speed = 1.0f;
        }

        if (!IsCritAnimationActive)
        {
            overlayCRIT.gameObject.SetActive(false);
            for (int i = 0; i < healthBarDefine.Count; i++)
            {
                if ((float)currentHealth / maxHealth >= healthBarDefine[i].healthThreshold)
                {
                    healthBarImage.sprite = healthBarDefine[i].sprite;
                    break;
                }
            }
        }
    }
}
