using System.Collections;
using UnityEngine;

public class CharacterBattleVisual : MonoBehaviour
{
    Animator PlayerBattleAnimator;
    [SerializeField] private SpriteRenderer mainBodyRenderer;
    [SerializeField] private SpriteRenderer mainHoldItemRenderer;
    [SerializeField] private SpriteRenderer[] overlayRenderer;
    [SerializeField] private SpriteRenderer[] mainRenderers;
    [SerializeField] private Light rightLight;

    Color flashStartColor = new Color(1, 0, 0, 1f);   // 初始红色（带透明）
    Color flashEndColor = new Color(1, 0, 0, 0f);     // 最终变为完全透明

    private const float FLASH_DURATION = 0.6f;
    private const float GUN_FIRE_LIGHT_DURATION = 0.2f;

    public BattleEntityBase battleEntity;

    private void Awake()
    {
        PlayerBattleAnimator = GetComponent<Animator>();

        if (overlayRenderer.Length > 0)
        {
            for (int i = 0; i <overlayRenderer.Length ; i++)
            {
                overlayRenderer[i].gameObject.SetActive(false);
                overlayRenderer[i].color = new Color(1, 0, 0, 1.0f);
            }
        }
    }

    public void RightLightOn()
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
}
