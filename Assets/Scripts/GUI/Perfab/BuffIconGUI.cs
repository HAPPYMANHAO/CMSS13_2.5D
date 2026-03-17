using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buffStacks;
    [SerializeField] private TextMeshProUGUI buffDuration;
    [SerializeField] private Image buffIcon;

    public void SetBuff(Sprite icon, int stacks,int duration, BuffBase buff)
    {
        buffIcon.sprite = icon;
        RefreshBuff(buff ,stacks, duration);
    }

    public void RefreshBuff(BuffBase buff ,int stacks, int duration)
    {
        buffDuration.text = buff.durationType switch
        {
            BuffBase.DurationType.Permanent => "∞",
            BuffBase.DurationType.Condition => "●",
            _ => duration.ToString(),
        };
        buffStacks.text = stacks.ToString();
    }
}
