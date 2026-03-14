using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buffStacks;
    [SerializeField] private TextMeshProUGUI buffDuration;
    [SerializeField] private Image buffIcon;

    public void SetBuff(Sprite icon, int stacks,int duration)
    {
        buffIcon.sprite = icon;
        RefreshBuff(stacks, duration);
    }

    public void RefreshBuff(int stacks, int duration)
    {
        buffDuration.text = duration.ToString();
        buffStacks.text = stacks.ToString();
    }
}
