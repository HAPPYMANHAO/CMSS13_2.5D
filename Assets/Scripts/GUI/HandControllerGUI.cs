using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandControllerGUI : MonoBehaviour
{
    [SerializeField] public Sprite activeSprite;
    [SerializeField] public Sprite disactiveSprite;
    [SerializeField] public Image holdItemImage;
    [SerializeField] public TextMeshProUGUI quantityText;
    [SerializeField] private Image handOccupiedImage;
    public Sprite holdItemSprite;
    private Color disableColor = new Color(1f, 1f, 1f, 0f);

    public Button handButton;
    public Image baseSprite;

    [Header("Animation Settings")]
    [SerializeField] private float itemEnterDuration = 0.3f;
    [SerializeField] private float itemEnterDistance = 50f;
    private const float HAND_UI_OFFSET = 80f;
    [SerializeField] private Ease itemEnterEase = Ease.OutBack;

    private void Start()
    {
        handOccupiedImage.color = disableColor;
    }

    private void Awake()
    {
        baseSprite = GetComponent<Image>();
        handButton = GetComponent<Button>();
    }

    public void EnableHandOccupiedImage()
    {
        handOccupiedImage.color = Color.white;
    }
    public void DisableHandOccupiedImage()
    {
        handOccupiedImage.color = disableColor;
    }

    public void EnableHoldItemSprite()
    {
        holdItemImage.color = Color.white;
    }
    public void DisableHoldItemSprite()
    {
        holdItemImage.color = disableColor;
    }

    public void UpdateQuantityDisplay(ItemInstance item)
    {
        if (item is StackableItemInstance stackable)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = stackable.currentQuantity.ToString();
        }
        else if (item is GunInstance gun)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = gun.currentAmmoCount.ToString() + "/" + gun.GunData.magazineCapacity;
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 播放物品进入手的动画（从下到上）
    /// </summary>
    public void PlayItemEnterAnimation()
    {
        if (holdItemImage == null) return;

        // 初始位置（从下方开始）
        Vector2 startPos = new Vector2(0, -itemEnterDistance);
        holdItemImage.rectTransform.anchoredPosition = startPos;

        // 执行动画移动到原点
        holdItemImage.rectTransform
            .DOAnchorPos(Vector2.zero, itemEnterDuration)
            .SetEase(itemEnterEase);
    }

    /// <summary>
    /// 设置物品图标并播放进入动画
    /// </summary>
    public void SetItemIconWithAnimation(Sprite icon)
    {
        if (holdItemImage == null) return;

        holdItemImage.sprite = icon;
        EnableHoldItemSprite();
        PlayItemEnterAnimation();
    }
}
