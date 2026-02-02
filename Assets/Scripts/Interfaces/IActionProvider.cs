using System.Collections.Generic;
using UnityEngine;

public interface IActionProvider
{
    IEnumerable<IBattleAction> GetAvailableActions();
}