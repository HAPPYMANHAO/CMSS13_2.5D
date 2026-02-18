using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHealthBarGUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image healthBarImage;
    [SerializeField] private UnityEngine.UI.Image _APBarImage;

    [SerializeField] public List<HealthBarEntry> healthBarDefine;
    [SerializeField] public List<HealthBarEntry> _APBarDefine;

    public EnemyBattleEntity owner;

    public void EnemyHealthBarBind(EnemyBattleEntity entity)
    {
        if (owner != null)
        {
            owner.OnHealthChanged -= HandleHealthUpdate;
            owner.OnApChanged -= HandleUpdateAP;
        }

        this.owner = entity;
        //绑定health bar 和AP
        owner.OnHealthChanged += HandleHealthUpdate;
        owner.OnApChanged += HandleUpdateAP;

        this.gameObject.SetActive(true);
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
        for (int i = 0; i < healthBarDefine.Count; i++)
        {
            if ((float)owner.currentHealth / owner.maxHealth >= healthBarDefine[i].healthThreshold)
            {
                healthBarImage.sprite = healthBarDefine[i].sprite;
                break;
            }
        }
    }

    private void UpdateAP()
    {
        for (int i = 0; i < _APBarDefine.Count; i++)
        {
            if ((float)owner.currentAP / owner.maxAP >= _APBarDefine[i].healthThreshold)
            {
                _APBarImage.sprite = _APBarDefine[i].sprite;
                break;
            }
        }
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
