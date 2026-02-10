using System.Collections.Generic;
using UnityEngine;

public interface IHoldable : IActionProvider
{
    
    [Header("Provided Actions")]
    [SerializeField] public List<ActionBase> providedActions { get; set; }
}
