using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class HealthBarControllerGUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image overlayCRIT;
    private Animator animatorCRIT;
    private UnityEngine.UI.Image healthBarImage;
    private TextMeshProUGUI APValue;

    public List<HealthBarEntry> healthBarDefine;

    public PartyBattleEntity owner;

    private bool IsCritAnimationActive = false;

    private void Awake()
    {
        healthBarImage = this.GetComponent<UnityEngine.UI.Image>();
        APValue = this.transform.Find("PlayerAPValue").GetComponent<TextMeshProUGUI>();
        animatorCRIT = overlayCRIT.GetComponent<Animator>();
    }

    public void HealthBarBind(PartyBattleEntity entity)
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= HandleHealthUpdate;
            owner.OnApChanged -= HandleUpdateAP;
        }

        this.owner = entity;
        //绑定health bar GUI和AP value GUI
        owner.OnHealthChanged += HandleHealthUpdate;
        owner.OnApChanged += HandleUpdateAP;

        this.gameObject.SetActive(true);
        overlayCRIT.gameObject.SetActive(false);
        UpdateHealth();
        UpdateAP();
    }

    private void HandleHealthUpdate()
    {
        UpdateHealth();
    }
    private void HandleUpdateAP()
    {
        UpdateAP();
    }

    public void UpdateHealth()
    {
        bool healthCRIT = owner.currentHealth < 0;//没有血条在0以下的精灵，血条CRIT条件始终设置为 < 0
        bool healthShock = owner.currentHealth < owner.healthCRITShock;

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
                if ((float)owner.currentHealth / owner.maxHealth >= healthBarDefine[i].healthThreshold)
                {
                    healthBarImage.sprite = healthBarDefine[i].sprite;
                    break;
                }
            }
        }
    }

    private void UpdateAP()
    {
        APValue.text = owner.currentAP.ToString() + "/" + owner.maxAP.ToString();
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= HandleHealthUpdate;
            owner.OnApChanged -= HandleUpdateAP;
        }
    }
}
