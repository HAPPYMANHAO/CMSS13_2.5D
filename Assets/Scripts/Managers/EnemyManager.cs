using System.Collections.Generic;
using UnityEngine;
using static EnemyInfo;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemiesInfo;
    [SerializeField] private List<CurrentEnemyInfo> currentEnemiesInfo;

    private static readonly string[] EnemyName =
    {
        "GiantLizard" // EnemyName[0] 
    };

    private Dictionary<string, EnemyInfo> enemyDatabase = new Dictionary<string, EnemyInfo>();

    private void Awake()
    {
        // 初始化字典Initialize dictionary
        foreach (var info in allEnemiesInfo)
        {
            string fullName = info.FullName; 
            if (!enemyDatabase.ContainsKey(fullName))
                enemyDatabase.Add(fullName, info);
        }

        AddEnemy(EnemyMaturityLevel.Young, EnemyName[0]);
    }

    public void AddEnemy(EnemyMaturityLevel level, string baseName)
    {
        string lookupName = $"{level} {baseName}";

        if (enemyDatabase.TryGetValue(lookupName, out EnemyInfo info))
        {
            currentEnemiesInfo.Add(new CurrentEnemyInfo(info));
        }
        else
        {
            Debug.LogError($"找不到敌人: {lookupName}");
        }
    }

    public List<CurrentEnemyInfo> GetCurrentEnemy()
    {
        return currentEnemiesInfo;
    }
}

[System.Serializable]
public class CurrentEnemyInfo
{
    public string memberName;
    public EnemyMaturityLevel enemyMaturityLevel;
    public int currentHealth;
    public int maxHealth;
    public int healthDead;
    public int maxAP;
    public int eachTurnRecoveredAP;
    public int currentAP;
    public int meleeStrength;
    public int rangedStrength;
    public GameObject enemyBattleVisualPerfab;

    public Dictionary<DamageType, ArmorStats> armorStats;

    public EntityAI entityAI;
    public CurrentEnemyInfo(EnemyInfo enemy)
    {
        this.enemyMaturityLevel = enemy.enemyMaturityLevel;
        this.memberName = enemy.enemyName;
        this.maxHealth = enemy.baseHealth;
        this.currentHealth = this.maxHealth;
        this.healthDead = enemy.healthDead;
        this.maxAP = enemy.MaxAP;
        this.eachTurnRecoveredAP = enemy.eachTurnRecoveredAP;
        this.currentAP = (maxAP / 2);
        this.meleeStrength = enemy.meleeStrength;
        this.rangedStrength = enemy.rangedStrenth;
        this.enemyBattleVisualPerfab = enemy.enemyBattleVisualPerfab;
        this.entityAI = enemy.entityAI;

        armorStats = enemy.GetArmorDictionary();
    }
}
