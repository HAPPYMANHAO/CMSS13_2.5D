using TMPro;
using Unity.VisualScripting;
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
}
