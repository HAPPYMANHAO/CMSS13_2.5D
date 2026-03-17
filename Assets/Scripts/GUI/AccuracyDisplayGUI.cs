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
        Hide();
    }

    public void UpdateAccuracy(float accuracy)
    {
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
        crosshairImage.transform.localScale = Vector3.one * crosshairScale;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void PlayFireFeedback(float accuracy)
    {
        Transform t = crosshairImage.transform;

        t.DOKill();

        float punchStrength = Mathf.Lerp(0.4f, 0.8f, 1f - accuracy);

        t.DOPunchScale(
            Vector3.one * punchStrength,
            0.2f,
            6,
            0.8f
        );
    }
}