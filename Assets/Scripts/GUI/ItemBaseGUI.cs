using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBaseGUI : MonoBehaviour
{
    [SerializeField] public Image itemIcon;
    private ItemBase _item;


    public void Setup(ItemBase item, Action<ItemBase> onClickCallback)
    {
        _item = item;
        itemIcon.sprite = item.icon;

        GetComponent<Button>().onClick.AddListener(() => onClickCallback(_item));
    }
}
