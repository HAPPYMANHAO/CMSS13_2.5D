using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyDisplayGUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Image crosshairImage;

    [Header("Color Settings")]
    [SerializeField] private Color highAccuracyColor = new Color(0.2f, 0.8f, 0.2f);   // 绿色
    [SerializeField] private Color mediumAccuracyColor = new Color(0.9f, 0.7f, 0.2f); // 黄色
    [SerializeField] private Color lowAccuracyColor = new Color(0.9f, 0.2f, 0.2f);    // 红色

    private void Start()
    {
        // 检查引用是否正确设置
        if (accuracyText == null)
            Debug.LogError("[AccuracyDisplayGUI] accuracyText 未设置！请在 Inspector 中设置引用。");
        if (crosshairImage == null)
            Debug.LogError("[AccuracyDisplayGUI] crosshairImage 未设置！请在 Inspector 中设置引用。");
        
        Hide();
    }

    private void OnDisable()
    {
        // 对象禁用时停止所有 DOTween 动画
        if (crosshairImage != null)
            crosshairImage.transform.DOKill();
    }

    public void UpdateAccuracy(float accuracy)
    {
        // 检查引用是否有效
        if (accuracyText == null || crosshairImage == null) return;

        // 更新文本
        accuracyText.text = $"{accuracy * 100:F0}%";

        // 颜色编码
        Color targetColor;
        if (accuracy >= 0.8f)
            targetColor = highAccuracyColor;
        else if (accuracy >= 0.5f)
            targetColor = mediumAccuracyColor;
        else
            targetColor = lowAccuracyColor;

        accuracyText.color = targetColor;

        // 准心大小（精准度越低越大）
        float crosshairScale = Mathf.Lerp(1.2f, 0.5f, accuracy);
        
        // 如果不在播放反馈动画，则更新准星大小
        if (!DOTween.IsTweening(crosshairImage.transform))
        {
            crosshairImage.transform.localScale = Vector3.one * crosshairScale;
        }
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void PlayFireFeedback(float accuracy)
    {
        // 检查对象是否激活
        if (!gameObject.activeSelf || crosshairImage == null) return;

        Transform t = crosshairImage.transform;

        // 停止之前的动画
        t.DOKill();

        // 重置缩放
        t.localScale = Vector3.one;

        // 根据准确度计算冲击强度（准确度越低，冲击越大）
        float punchStrength = Mathf.Lerp(0.4f, 0.8f, 1f - accuracy);

        // 播放冲击动画
        t.DOPunchScale(
            Vector3.one * punchStrength,
            0.2f,
            6,
            0.8f
        ).SetUpdate(true); // 使用 SetUpdate(true) 确保动画在 Time.timeScale = 0 时也能播放
    }
}