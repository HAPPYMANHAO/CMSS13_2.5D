using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;



public class HealthBarControllerGUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image overlayCRIT;
    private Animator animatorCRIT;
    private UnityEngine.UI.Image healthBarImage;
    [SerializeField] private TextMeshProUGUI APValue;
    [SerializeField] private TextMeshProUGUI HPValue;
    [SerializeField] private UnityEngine.UI.Image profile;
    [SerializeField] private TextMeshProUGUI charaterName;

    public List<HealthBarEntry> healthBarDefine;

    public IHealthAndAPComponent owner;

    private bool IsCritAnimationActive = false;

    private void Awake()
    {
        healthBarImage = this.GetComponent<UnityEngine.UI.Image>();
        animatorCRIT = overlayCRIT.GetComponent<Animator>();
    }

    public void UpdateHealthBar()
    {

            UpdateHealth(owner.currentHealth, owner.maxHealth, owner.healthCRITShock);
            UpdateAP();
    }

    public void HealthBarBind(PartyBattleEntity entity)
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= UpdateHealthBar;
            owner.OnApChanged -= HandleUpdateAP;
        }

        this.owner = entity;
        owner.OnHealthChanged += UpdateHealthBar;
        owner.OnApChanged += HandleUpdateAP;

        profile.sprite = entity.charaterProfile;
        charaterName.text = entity.memberName;

        this.gameObject.SetActive(true);
        overlayCRIT.gameObject.SetActive(false);
        UpdateHealthBar();
    }

    public void HealthBarBind(CurrentPartyMemberInfo memberInfo)
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= UpdateHealthBar;
            owner.OnApChanged -= HandleUpdateAP;
        }

        
        this.owner = memberInfo;
        owner.OnHealthChanged += UpdateHealthBar;
        APValue.text = $"<size=100%>{"--"}</size>\n<size=80%>/{"--"}</size>";//不需要在overworld显示AP

        charaterName.text = memberInfo.memberName;
        profile.sprite = memberInfo.charaterProfile;
        
        this.gameObject.SetActive(true);
        
        UpdateHealthBar();
    }

    private void HandleUpdateAP()
    {
        UpdateAP();
    }

    public void UpdateHealth(int currentHealth, int maxHealth, int healthCRITShock)
    {
        bool healthCRIT = currentHealth < 0;//没有血条在0以下的精灵，血条CRIT条件始终设置为 < 0
        bool healthShock = currentHealth < healthCRITShock;  
        if (GameStateManager.instance.currentGameState == GameState.Battle)
        {
            HPValue.text = $"<size=100%>{owner.currentHealth}</size>\n<size=80%>/{owner.maxHealth}</size>";
        }
        else if (GameStateManager.instance.currentGameState == GameState.Overworld)
        {
            HPValue.text = $"<size=100%>{owner.currentHealth}</size>\n<size=80%>/{owner.maxHealth}</size>";
            APValue.text = $"<size=100%>{"--"}</size>\n<size=80%>/{"--"}</size>";//不需要在overworld显示AP
        }
        if (healthCRIT != IsCritAnimationActive)
        {
            IsCritAnimationActive = healthCRIT;
            overlayCRIT.gameObject.SetActive(true);
        }

        if (IsCritAnimationActive)
        {
            animatorCRIT.speed = healthShock ? 2.0f : 1.0f;
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
 

    private void UpdateAP()
    {
        if(owner is CurrentPartyMemberInfo)
        {
            APValue.text = $"<size=100%>{"--"}</size>\n<size=80%>/{"--"}</size>";//不需要在overworld显示AP
        }
        else
        {
            APValue.text = $"<size=100%>{owner.currentAP}</size>\n<size=80%>/{owner.maxAP}</size>";
        } 
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= UpdateHealthBar;
            owner.OnApChanged -= HandleUpdateAP;
        }
    }

    private void OnDisable()
    {
        if (owner != null) owner.OnHealthChanged -= UpdateHealthBar;
    }
}
