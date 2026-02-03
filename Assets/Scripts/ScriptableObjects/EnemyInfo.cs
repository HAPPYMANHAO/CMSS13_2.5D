using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]

public class EnemyInfo : ScriptableObject
{
    public EnemyMaturityLevel enemyMaturityLevel;
    public string enemyName;
    public int baseHealth;
    public int MaxAP;
    public int eathTurnRecoveredAP;
    public int meleeStrenth;
    public int healthDead = 0;
    public GameObject enemyBattleVisualPerfab;

    public List<ArmorStats> armorStats = new List<ArmorStats>();

    public string FullName => $"{enemyMaturityLevel} {enemyName}";

    public Dictionary<ArmorType, ArmorStats> GetArmorDictionary()
    {
        Dictionary<ArmorType, ArmorStats> dict = new Dictionary<ArmorType, ArmorStats>();
        foreach (var armor in armorStats)
        {
            dict[armor.armorType] = armor;
        }
        return dict;
    }
}


