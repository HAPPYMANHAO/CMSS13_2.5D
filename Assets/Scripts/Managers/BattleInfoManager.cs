using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BattleInfoManager : MonoBehaviour
{
    [Header("SpawnPoints")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
   
    private List<IBattleEntity> allBattleEntities = new List<IBattleEntity>();
    [Header("BattleEntity")]
    [SerializeField] private List<PartyBattleEntity> partyBattleEntities = new List<PartyBattleEntity>();
    [SerializeField] private List<EnemyBattleEntity> enemyBattleEntities = new List<EnemyBattleEntity>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    
    private BattleVisualGUI battleVisualGUI;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();

        battleVisualGUI = FindFirstObjectByType<BattleVisualGUI>();
    }

    private void Start()
    {
        SpawnPartyEntity();
        SpawnEnemyEntity();
    }

    //----------------------------SpawnEntity----------------------------//
    //----------------Spawn Party Entity/1
    private void SpawnPartyEntity()
    {
        List<CurrentPartyMemberInfo> currentParty = new List<CurrentPartyMemberInfo>();
        currentParty = partyManager.GetCurrentPartyMember();

        for (int i = 0; i < currentParty.Count; i++)
        {
            PartyBattleEntity partyBattleEntity = new PartyBattleEntity(currentParty[i]);
            partyBattleEntity.isPlayerFaction = true;

            CharacterBattleVisual tempBattleVisual = 
                Instantiate(currentParty[i].memberBattleVisualPerfab, partySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = partyBattleEntity;
            partyBattleEntity.battleVisual = tempBattleVisual;

            battleVisualGUI.BindHealthBar(partyBattleEntity);

            allBattleEntities.Add(partyBattleEntity);
            partyBattleEntities.Add(partyBattleEntity);
        }
    }
    //----------------Spawn Enemy Entity/2
    private void SpawnEnemyEntity()
    {
        List<CurrentEnemyInfo> currentEnemy = new List<CurrentEnemyInfo>();
        currentEnemy = enemyManager.GetCurrentEnemy();

        for (int i = 0; i < currentEnemy.Count; i++)
        {
            EnemyBattleEntity enemyBattleEntity = new EnemyBattleEntity(currentEnemy[i]);
            enemyBattleEntity.isPlayerFaction = false;

            CharacterBattleVisual tempBattleVisual =
                Instantiate(currentEnemy[i].enemyBattleVisualPerfab, enemySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = enemyBattleEntity;
            enemyBattleEntity.battleVisual = tempBattleVisual;

            allBattleEntities.Add(enemyBattleEntity);
            enemyBattleEntities.Add(enemyBattleEntity);
        }
    }
}

//----------------------------Class----------------------------//
//----------------BattleEntity Class/
[System.Serializable]
public class PartyBattleEntity : BattleEntityBase
{
    public int healthCRIT;
    public int healthCRITShock;
    public int healthDead;

    public int currentLevel;

    public CharacterBattleVisual battleVisual;

    public Dictionary<SkillType, int> skills;

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

    public override void ExecuteAction(ActionBase battleAction, BattleEntityBase target)
    {
    }

    public bool PartyMemberIsCrit(PartyBattleEntity entity)
    {
        return entity.currentHealth <= entity.healthCRIT;
    }

    public bool PartyMemberIsCritShock(PartyBattleEntity entity)
    {
        return entity.currentHealth <= entity.healthCRITShock;
    }
}
[System.Serializable]
public class EnemyBattleEntity : BattleEntityBase
{
    public EnemyMaturityLevel currentEnemyLevel;
    public int healthDead = 0;
    public int meleeStrength;

    public CharacterBattleVisual battleVisual;

    public EnemyBattleEntity(CurrentEnemyInfo enemyInfo)
    {
        memberName = enemyInfo.memberName;
        currentEnemyLevel = enemyInfo.enemyMaturityLevel; 
        maxHealth = enemyInfo.maxHealth;
        maxAP = enemyInfo.maxAP;
        eachTurnRecoveredAP = enemyInfo.eachTurnRecoveredAP;
        currentAP = enemyInfo.currentAP;
        meleeStrength = enemyInfo.meleeStrength;

        armorStats = enemyInfo.armorStats;
    }

    public override void ExecuteAction(ActionBase battleAction, BattleEntityBase target)
    {
    }
}
