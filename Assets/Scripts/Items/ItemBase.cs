using UnityEngine;

[CreateAssetMenu(menuName = "New Item")]
public abstract class ItemBase : ScriptableObject
{
    public string itemName;

    public Sprite icon;
}
