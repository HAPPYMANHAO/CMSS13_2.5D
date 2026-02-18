using System.Collections.Generic;
using UnityEngine;

public interface IHoldable : IActionProvider
{
    List<ActionBase> providedActions { get; set; }
}
