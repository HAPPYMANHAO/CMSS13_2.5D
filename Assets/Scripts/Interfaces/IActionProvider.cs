using System.Collections.Generic;
using UnityEngine;

public interface IActionProvider
{
    ActionBase GetCurrentActions();
}