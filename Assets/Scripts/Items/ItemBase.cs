using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;
}
