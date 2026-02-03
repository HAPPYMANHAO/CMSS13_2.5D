using UnityEngine;
using UnityEngine.UI;

public class HandControllerGUI : MonoBehaviour
{
    [SerializeField] public Sprite acitveSprite;
    [SerializeField] public Sprite disacitveSprite;

    public Button handButton;
    public Image baseSprite;

    private void Awake()
    {
        baseSprite = GetComponent<Image>();
        handButton = GetComponent<Button>();
    }
}
