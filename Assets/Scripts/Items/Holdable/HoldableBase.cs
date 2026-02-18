using System.Collections.Generic;
using UnityEngine;

public abstract class HoldableBase : ItemBase, IHoldable
{
    [Header("Provided Actions")]
    [SerializeField] public List<ActionBase> _providedActions;

    public List<ActionBase> providedActions
    {
        get => _providedActions;
        set => _providedActions = value;
    }

    public abstract ActionBase GetCurrentActions();
}
