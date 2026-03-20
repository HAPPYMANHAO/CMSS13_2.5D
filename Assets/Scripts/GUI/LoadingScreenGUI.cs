using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenGUI : MonoBehaviour
{
    public static LoadingScreenGUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI loadingTipText;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string[] loadingTips;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始隐藏
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        else
            Debug.LogError("[LoadingScreenGUI] CanvasGroup 未设置！请在 Inspector 中设置。");
            
        if (progressBar == null)
            Debug.LogWarning("[LoadingScreenGUI] ProgressBar (Image) 未设置！");
            
        if (progressText == null)
            Debug.LogWarning("[LoadingScreenGUI] ProgressText (TextMeshProUGUI) 未设置！");
            
        if (loadingTipText == null)
            Debug.LogWarning("[LoadingScreenGUI] LoadingTipText (TextMeshProUGUI) 未设置！");
            
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示加载界面
    /// </summary>
    public void Show(float customFadeDuration = -1f, System.Action onComplete = null)
    {
        // 检查关键组件
        if (canvasGroup == null)
        {
            Debug.LogError("[LoadingScreenGUI] CanvasGroup 未设置！");
            onComplete?.Invoke();
            return;
        }

        gameObject.SetActive(true);
        
        // 确保初始状态正确
        canvasGroup.alpha = 0f;

        // 随机显示加载提示
        if (loadingTipText != null)
        {
            if (loadingTips != null && loadingTips.Length > 0)
            {
                loadingTipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
            }
            else
            {
                loadingTipText.text = "Loading...";
            }
        }

        // 重置进度
        UpdateProgress(0f);

        // 淡入动画
        float actualFadeDuration = customFadeDuration > 0 ? customFadeDuration : fadeDuration;
        canvasGroup.DOKill();
        
        if (onComplete != null)
        {
            // 如果有回调，等待淡入完成
            canvasGroup.DOFade(1f, actualFadeDuration)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            // 没有回调，直接播放动画
            canvasGroup.DOFade(1f, actualFadeDuration).SetUpdate(true);
        }
    }

    /// <summary>
    /// 隐藏加载界面
    /// </summary>
    public void Hide(float customFadeDuration = -1f, System.Action onComplete = null)
    {
        // 检查关键组件
        if (canvasGroup == null)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        float actualFadeDuration = customFadeDuration > 0 ? customFadeDuration : fadeDuration;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, actualFadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 更新加载进度
    /// </summary>
    public void UpdateProgress(float progress)
    {
        // 检查对象是否激活
        if (gameObject == null || !gameObject.activeInHierarchy) return;
        
        progress = Mathf.Clamp01(progress);

        // 更新进度条
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        // 更新文本
        if (progressText != null)
        {
            progressText.text = $"{progress * 100:F0}%";
        }
    }

    /// <summary>
    /// 强制立即隐藏（用于紧急情况）
    /// </summary>
    public void ForceHide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
    }
}