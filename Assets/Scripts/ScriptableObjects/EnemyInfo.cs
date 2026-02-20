using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]

public class EnemyInfo : ScriptableObject
{
    public EnemyMaturityLevel enemyMaturityLevel;
    public string enemyName;
    public int baseHealth;
    public int MaxAP;
    public int eachTurnRecoveredAP;
    public int meleeStrength;
    public int rangedStrength;
    [Range(0f, 1f)] public float armorPenetration;
    public int healthDead = 0;
    public GameObject enemyBattleVisualPerfab;

    public List<ArmorStats> armorStats = new List<ArmorStats>();
    public List<DamageResistanceStats> damageResistanceStats = new List<DamageResistanceStats>();

    public EntityAI entityAI;

    public string FullName => $"{enemyMaturityLevel} {enemyName}";

    public Dictionary<DamageType, ArmorStats> GetArmorDictionary()
    {
        Dictionary<DamageType, ArmorStats> dict = new Dictionary<DamageType, ArmorStats>();
        foreach (var armor in armorStats)
        {
            dict[armor.armorType] = armor;
        }
        return dict;
    }
    public Dictionary<DamageType, float> GetDamageResistanceDictionary()
    {
        Dictionary<DamageType, float> dict = new Dictionary<DamageType, float>();
        foreach (DamageResistanceStats damageResistanceStat in damageResistanceStats)
        {
            dict[damageResistanceStat.resistType] = damageResistanceStat.damageResist;
        }
        return dict;
    }

    //----------------------OnValidate----------------------//

    private bool armorInitialized = false;
    private bool damageResistanceInitialized = false;
    private void OnValidate()
    {
        if (!armorInitialized)
        {
            InitializeAromr();
            armorInitialized = true;
        }
        if (!damageResistanceInitialized)
        {
            InitializeDamageResistan();
            damageResistanceInitialized = true;
        }

        DamageType[] allDamgaeType = (DamageType[])System.Enum.GetValues(typeof(DamageType));

        if (armorStats.Count != allDamgaeType.Length)
        {
            Dictionary<DamageType, int> existingAromrValue = new Dictionary<DamageType, int>();
            foreach (var _aromr in armorStats)
            {
                existingAromrValue[_aromr.armorType] = _aromr.armorValue;
            }
            Dictionary<DamageType, float> existingArmorStrength = new Dictionary<DamageType, float>();
            foreach (var _aromr in armorStats)
            {
                existingArmorStrength[_aromr.armorType] = _aromr.armorIntegrity;
            }

            armorStats.Clear();

            foreach (var _aromr in allDamgaeType)
            {
                armorStats.Add(new ArmorStats
                {
                    armorType = _aromr,
                    armorIntegrity = existingArmorStrength.ContainsKey(_aromr) ? existingArmorStrength[_aromr] : 0f,
                    armorValue = existingAromrValue.ContainsKey(_aromr) ? existingAromrValue[_aromr] : 0,
                });
            }
        }

        if (damageResistanceStats.Count != allDamgaeType.Length)
        {
            Dictionary<DamageType, float> existingResistancealue = new Dictionary<DamageType, float>();
            foreach (DamageResistanceStats _damageResist in damageResistanceStats)
            {
                existingResistancealue[_damageResist.resistType] = _damageResist.damageResist;
            }

            damageResistanceStats.Clear();

            foreach (var _damageResist in allDamgaeType)
            {
                damageResistanceStats.Add(new DamageResistanceStats
                {
                    resistType = _damageResist,
                    damageResist = existingResistancealue.ContainsKey(_damageResist) ? existingResistancealue[_damageResist] : 0,
                });
            }
        }
    }

    private void InitializeAromr()
    {
        armorStats.Clear();

        DamageType[] allDamgaeType = (DamageType[])System.Enum.GetValues(typeof(DamageType));

        foreach (var damageType in allDamgaeType)
        {
            armorStats.Add(new ArmorStats
            {
                armorType = damageType,
                armorIntegrity = 0,
                armorValue = 0,
            });
        }
    }

    private void InitializeDamageResistan()
    {
        damageResistanceStats.Clear();

        DamageType[] allDamgaeType = (DamageType[])System.Enum.GetValues(typeof(DamageType));

        foreach (var damageType in allDamgaeType)
        {
            damageResistanceStats.Add(new DamageResistanceStats
            {
                resistType = damageType,
                damageResist = 0f,
            });
        }
    }
}


