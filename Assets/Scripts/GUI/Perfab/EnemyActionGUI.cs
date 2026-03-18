using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyActionGUI : MonoBehaviour
{
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI UITextMeshPro;

    public Image GetImageUI()
    {
        return profile;
    }
    public TextMeshProUGUI GetTextUI()
    {
        return UITextMeshPro;
    }
    public void Setup(EnemyBattleEntity enemy, ActionBase action)
    {
        UITextMeshPro.text = enemy.memberName;
        profile.sprite = enemy.charaterProfile;
    }
}
