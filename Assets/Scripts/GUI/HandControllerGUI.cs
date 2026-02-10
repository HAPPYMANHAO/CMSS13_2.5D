using UnityEngine;
using UnityEngine.UI;

public class HandControllerGUI : MonoBehaviour
{
    [SerializeField] public Sprite activeSprite;
    [SerializeField] public Sprite disactiveSprite;
    [SerializeField] public Image holdItemImage;
    public Sprite holdItemSprite;
    private Color disableColor = new Color(1f, 1f, 1f, 0f);

    public Button handButton;
    public Image baseSprite;

    private void Awake()
    {
        baseSprite = GetComponent<Image>();
        handButton = GetComponent<Button>();
    }

    public void EnableHoldItemSprite()
    {
        holdItemImage.color = Color.white;
    }
    public void DisableHoldItemSprite()
    {
        holdItemImage.color = disableColor;
    }
}
