using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBattleVisual : MonoBehaviour
{
    Animator characterAnimator;
    [SerializeField] private SpriteRenderer mainBodyRenderer;
    
    [SerializeField] private SpriteRenderer[] overlayRenderer;
    [SerializeField] private SpriteRenderer[] mainRenderers;
    [SerializeField] private Light rightLight;
    [SerializeField] private GameObject buffContainer;

    [Header("prefab")]
    [SerializeField] private BuffIconGUI buffIconPrefab;

    [Header("For Player Praty")]
    [SerializeField] private SpriteRenderer mainHoldItemRenderer;
    [SerializeField] private ItemDisplayer itemDisplayer; 

    Color flashStartColor = new Color(1, 0, 0, 1f);   // 初始红色（带透明）
    Color flashEndColor = new Color(1, 0, 0, 0f);     // 最终变为完全透明

    private const float FLASH_DURATION = 0.6f;
    private const float GUN_FIRE_LIGHT_DURATION = 0.2f;
    private const float DEAD_ANIMATION_DURATION = 3.0f;

    public BattleEntityBase battleEntity;

    private const string IS_DEAD_PARAM = "IsDead";

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();

        if (overlayRenderer.Length > 0)
        {
            for (int i = 0; i <overlayRenderer.Length ; i++)
            {
                overlayRenderer[i].gameObject.SetActive(false);
                overlayRenderer[i].color = new Color(1, 0, 0, 1.0f);
            }
        }
    }

    private void Start()
    {
        battleEntity.OnEntityDeath += HandleEntityDead;
        battleEntity.buffComponent.OnBuffChanged += HandleBuffChanged;
        battleEntity.buffComponent.OnBuffDataChanged += HandleBuffDateChanged;
    }

    private void OnDestroy()
    {
        if (battleEntity != null)
        {
            battleEntity.OnEntityDeath -= HandleEntityDead;
            battleEntity.buffComponent.OnBuffChanged -= HandleBuffChanged;
            battleEntity.buffComponent.OnBuffDataChanged -= HandleBuffDateChanged;
        }
    }

    public void RightLightOn()//gun fire
    {
        StartCoroutine(RightLightRoutine());
    }

    public void PlayHurt()
    {
        if (mainBodyRenderer == null || overlayRenderer.Length <= 0) return;

        StartCoroutine(RedFlashRoutine());
    }

    private IEnumerator RedFlashRoutine()
    {
        float currentTimer = 0;

        for (int i = 0; i < overlayRenderer.Length; i++)
        {
            overlayRenderer[i].gameObject.SetActive(true);
            overlayRenderer[i].sprite = mainRenderers[i].sprite;
        }

        while (currentTimer < FLASH_DURATION)
        {
            currentTimer += Time.deltaTime;
            float progress = currentTimer / FLASH_DURATION;
            for (int i = 0; i < overlayRenderer.Length; i++)
            {
                overlayRenderer[i].color = Color.Lerp(flashStartColor, flashEndColor, progress);
            }
            yield return null;         
        }

        for (int i = 0; i < overlayRenderer.Length; i++)
        {
            overlayRenderer[i].gameObject.SetActive(false);
        }     
    }

    private IEnumerator RightLightRoutine()
    {
        rightLight.gameObject.SetActive(true);
        yield return new WaitForSeconds(GUN_FIRE_LIGHT_DURATION);
        rightLight.gameObject.SetActive(false);
    }

    private void HandleEntityDead(BattleEntityBase deadEntity)
    {
        characterAnimator.SetBool(IS_DEAD_PARAM, true);
        Destroy(gameObject, DEAD_ANIMATION_DURATION);  
    }

    private void HandleBuffChanged()
    {
        // 1. 清理旧的图标（或者使用对象池优化）
        foreach (Transform child in buffContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. 遍历当前所有活跃的 Buff
        foreach (var buff in battleEntity.buffComponent.ActiveBuffs)
        {
            // 3. 实例化预制体并填充数据
            BuffIconGUI icon = Instantiate(buffIconPrefab);
            icon.transform.SetParent(buffContainer.transform, false);
            icon.SetBuff(buff.buffData.icon, buff.currentStacks, buff.remainingTurns);
        }
    }

    private void HandleBuffDateChanged()
    {
        foreach (var buff in battleEntity.buffComponent.ActiveBuffs)
        {
            foreach(BuffIconGUI child in buffContainer.transform)
            {
                if(child.buffIcon.sprite = buff.buffData.icon)
                {
                    child.RefreshBuff(buff.currentStacks, buff.remainingTurns);
                }                     
            }     
        }
    }

    public void ActiveItemDisplyer(Sprite sprite)
    {
        if (itemDisplayer.gameObject.activeSelf)
        {
            itemDisplayer.gameObject.SetActive(false);
        }

        itemDisplayer.spriteRenderer.sprite = sprite;

        itemDisplayer.gameObject.SetActive(true);
    }
}
