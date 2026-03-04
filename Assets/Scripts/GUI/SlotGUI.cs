using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] public EquipmentSlotType slotType;
    [SerializeField] public Image icon;       
    [SerializeField] public Button button;

    private Color disableColor = new Color(1f, 1f, 1f, 0f);

    public void EnableItemSprite()
    {
        icon.color = Color.white;
    }
    public void DisableItemSprite()
    {
        icon.color = disableColor;
    }
}
