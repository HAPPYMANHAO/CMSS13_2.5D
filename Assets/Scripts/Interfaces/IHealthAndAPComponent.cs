using System;
using UnityEngine;

public interface IHealthAndAPComponent 
{
    string memberName { get; set; }
    int currentHealth { get; set; }
    int maxHealth { get; set; }
    int currentAP { get; set; }
    int maxAP { get; set; }
    int healthCRITShock { get; set; }

    public event Action OnApChanged;
    public event Action OnHealthChanged;
}
