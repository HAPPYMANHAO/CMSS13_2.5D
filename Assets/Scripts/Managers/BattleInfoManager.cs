using System.Collections.Generic;
using UnityEngine;

public class BattleInfoManager : MonoBehaviour
{
    [Header("SpawnPoints")]
    [SerializeField] private Transform[] PartySpawnPoints;
    [SerializeField] private Transform[] EnemySpawnPoints;

    [Header("BattleEntity")]
    [SerializeField] private List<IBattleEntity> allBattleEntities = new List<IBattleEntity>();
    [SerializeField] private List<PartyBattleEntity> partyBattleEntities = new List<PartyBattleEntity>();
    [SerializeField] private List<EnemyBattleEntity> enemyBattleEntities = new List<EnemyBattleEntity>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
    }

    private void Start()
    {
        SpawnPartyEntity();
        SpawnEnemyEntity();
    }

    private void SpawnPartyEntity()
    {
        List<CurrentPartyMemberInfo> currentParty = new List<CurrentPartyMemberInfo>();
        currentParty = partyManager.GetCurrentPartyMember();

        for (int i = 0; i < currentParty.Count; i++)
        {
            PartyBattleEntity partyBattleEntity = new PartyBattleEntity(currentParty[i]); 

            allBattleEntities.Add(partyBattleEntity);
            partyBattleEntities.Add(partyBattleEntity);
        }
    }

    private void SpawnEnemyEntity()
    {
        List<CurrentEnemyInfo> currentEnemy = new List<CurrentEnemyInfo>();
        currentEnemy = enemyManager.GetCurrentEnemy();

        for (int i = 0; i < currentEnemy.Count; i++)
        {
            EnemyBattleEntity enemyBattleEntity = new EnemyBattleEntity(currentEnemy[i]);

            allBattleEntities.Add(enemyBattleEntity);
            enemyBattleEntities.Add(enemyBattleEntity);
        }
    }
}

[System.Serializable]
public class PartyBattleEntity : IBattleEntity
{
    public string memberName;
    public int currentLevel;
    public int currentHealth;
    public int maxHealth;
    public int maxAP;
    public int eachTurnRecoveredAP;
    public int currentAP { get; set; }

    public int healthCRIT;
    public int healthCRITShock;
    public int healthDead;

    public bool isPlayerFaction = true;

    public Dictionary<SkillType, int> skills;
    public Dictionary<DamageType, ArmorStats> armorStats;

    public PartyBattleEntity(CurrentPartyMemberInfo memberInfo)
    {
        memberName = memberInfo.memberName;
        currentLevel = memberInfo.currentLevel;
        currentHealth = memberInfo.currentHealth;
        maxHealth = memberInfo.maxHealth;
        maxAP = memberInfo.maxAP;
        eachTurnRecoveredAP = memberInfo.eachTurnRecoveredAP;
        currentAP = memberInfo.currentAP;

        healthCRIT = memberInfo.healthCRIT;
        healthCRITShock = memberInfo.healthCRITShock;
        healthDead = memberInfo.healthDead;

        skills = memberInfo.skills;
        armorStats = memberInfo.armorStats;
    }

    public void ExecuteAction(IBattleAction battleAction, IBattleEntity target)
    {
    }

    public void TakeDamage(int amount, DamageType type)
    {
        
    }
}
[System.Serializable]
public class EnemyBattleEntity : IBattleEntity
{
    public string memberName;
    public EnemyMaturityLevel currentEnemyLevel;
    public int currentHealth;
    public int maxHealth;
    public int maxAP;
    public int eachTurnRecoveredAP;
    public int meleeeStrenth;
    public int currentAP { get; set; }

    public int healthDead = 0;

    public bool isPlayerFaction = false;

    public Dictionary<DamageType, ArmorStats> armorStats;

    public EnemyBattleEntity(CurrentEnemyInfo enemyInfo)
    {
        memberName = enemyInfo.memberName;
        currentEnemyLevel = enemyInfo.enemyMaturityLevel; 
        maxHealth = enemyInfo.maxHealth;
        maxAP = enemyInfo.maxAP;
        eachTurnRecoveredAP = enemyInfo.eachTurnRecoveredAP;
        currentAP = enemyInfo.currentAP;
        meleeeStrenth = enemyInfo.meleeStrenth;

        armorStats = enemyInfo.armorStats;
    }

    public void ExecuteAction(IBattleAction battleAction, IBattleEntity target)
    {
    }

    public void TakeDamage(int amount, DamageType type)
    {

    }
}
