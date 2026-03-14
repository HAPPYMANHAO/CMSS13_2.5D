using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }

    private void OnDestroy()
    {
        if (battleEntity != null)
        {
            battleEntity.OnEntityDeath -= HandleEntityDead;
            battleEntity.buffComponent.OnBuffChanged -= HandleBuffChanged;
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

    private Dictionary<BuffInstance, BuffIconGUI> buffIconMap = new Dictionary<BuffInstance, BuffIconGUI>();
    private Queue<BuffIconGUI> iconPool = new Queue<BuffIconGUI>();

    private void HandleBuffChanged()
    {
        // 1. 标记所有现有图标为未使用
        HashSet<BuffIconGUI> unusedIcons = new HashSet<BuffIconGUI>(buffIconMap.Values);

        // 2. 更新或创建图标
        foreach (var buff in battleEntity.buffComponent.ActiveBuffs)
        {
            BuffIconGUI icon;

            if (buffIconMap.TryGetValue(buff, out icon))
            {
                // 已存在，刷新
                icon.RefreshBuff(buff.currentStacks, buff.remainingTurns);
                unusedIcons.Remove(icon);
            }
            else
            {
                // 新buff，从池中获取或创建
                icon = GetOrCreateIcon();
                icon.SetBuff(buff.buffData.icon, buff.currentStacks, buff.remainingTurns);
                buffIconMap[buff] = icon;
            }
        }

        // 3. 回收未使用的图标
        foreach (var unusedIcon in unusedIcons)
        {
            ReturnIconToPool(unusedIcon);
            // 从map中移除
            var buffToRemove = buffIconMap.FirstOrDefault(x => x.Value == unusedIcon).Key;
            if (buffToRemove != null)
                buffIconMap.Remove(buffToRemove);
        }
    }

    private BuffIconGUI GetOrCreateIcon()
    {
        if (iconPool.Count > 0)
        {
            var icon = iconPool.Dequeue();
            icon.gameObject.SetActive(true);
            return icon;
        }

        var newIcon = Instantiate(buffIconPrefab, buffContainer.transform);
        return newIcon;
    }

    private void ReturnIconToPool(BuffIconGUI icon)
    {
        icon.gameObject.SetActive(false);
        iconPool.Enqueue(icon);
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
