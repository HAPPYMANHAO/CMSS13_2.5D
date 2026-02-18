using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBaseGUI : MonoBehaviour
{
    [SerializeField] public Image itemIcon;
    [SerializeField] public TextMeshProUGUI quantityText;
    private ItemInstance _item;

    /// <summary>
    /// 初始化 UI 格。
    /// Initializes the UI slot.
    /// </summary>
    public void Setup(ItemInstance item, Action<ItemInstance> onClickCallback)
    {
        _item = item;

        itemIcon.sprite = item.Icon;

        // 可堆叠物品显示数量
        if (item is StackableItemInstance stackable)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = stackable.currentQuantity.ToString();
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => onClickCallback(_item));
    }

    /// <summary>
    /// 刷新数量显示（不重建整个格，用于轻量刷新）。
    /// Refreshes the quantity display without rebuilding the entire slot.
    /// </summary>
    public void RefreshQuantity()
    {
        if (_item is StackableItemInstance stackable)
            quantityText.text = stackable.currentQuantity.ToString();
    }
}
