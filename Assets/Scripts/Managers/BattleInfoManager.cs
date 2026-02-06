using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BattleInfoManager : MonoBehaviour
{
    [Header("SpawnPoints")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    public List<BattleEntityBase> allBattleEntities = new List<BattleEntityBase>();
    public IEnumerable<PartyBattleEntity> PartyEntities =>
        allBattleEntities.OfType<PartyBattleEntity>();
    public IEnumerable<EnemyBattleEntity> EnemyEntities =>
        allBattleEntities.OfType<EnemyBattleEntity>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    
    private BattleVisualGUI battleVisualGUI;

    private int currentPlayerEntity;

    [SerializeField] ActionBase testAction;

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

        currentPlayerEntity = 0;  
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

            partyBattleEntity.entityFaction = Faction.Survivor;

            CharacterBattleVisual tempBattleVisual = 
                Instantiate(currentParty[i].memberBattleVisualPerfab, partySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = partyBattleEntity;
            partyBattleEntity.battleVisual = tempBattleVisual;

            battleVisualGUI.BindHealthBar(partyBattleEntity);

            allBattleEntities.Add(partyBattleEntity);
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

            enemyBattleEntity.entityFaction = Faction.Enemy;

            CharacterBattleVisual tempBattleVisual =
                Instantiate(currentEnemy[i].enemyBattleVisualPerfab, enemySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = enemyBattleEntity;
            enemyBattleEntity.battleVisual = tempBattleVisual;

            allBattleEntities.Add(enemyBattleEntity);
        }
    }

    //----------------------------GetEntitiesByFaction----------------------------//
    public List<T> GetEntitiesByFaction<T>(Faction faction) where T : BattleEntityBase
    {
        return allBattleEntities.OfType<T>()
            .Where(entity => entity.entityFaction == faction)
            .ToList();
    }
    //----------------------------Target----------------------------//
    public void PlayerComfirmTarget(BattleEntityBase entity)
    {
        BattleEntityBase[] targetEntities = { entity };

        allBattleEntities[currentPlayerEntity].ExecuteAction(testAction, targetEntities);
    }
}

//----------------------------Class----------------------------//
//----------------BattleEntity Class/
[System.Serializable]
public class PartyBattleEntity : BattleEntityBase
{
    public int healthCRIT;
    public int healthCRITShock;

    public int currentLevel;

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


    public bool PartyMemberIsCrit()
    {
        return currentHealth <= healthCRIT;
    }

    public bool PartyMemberIsCritShock()
    {
        return currentHealth <= healthCRITShock;
    }
}
[System.Serializable]
public class EnemyBattleEntity : BattleEntityBase
{
    public EnemyMaturityLevel currentEnemyLevel;
    public int meleeStrength;

    public EnemyBattleEntity(CurrentEnemyInfo enemyInfo)
    {
        memberName = enemyInfo.memberName;
        currentEnemyLevel = enemyInfo.enemyMaturityLevel;
        healthDead = 0;//敌人会在生命值归零时被击败 enemy will be defeated once their HP reach zero.
        maxHealth = enemyInfo.maxHealth;
        currentHealth = enemyInfo.currentHealth;
        maxAP = enemyInfo.maxAP;
        eachTurnRecoveredAP = enemyInfo.eachTurnRecoveredAP;
        currentAP = enemyInfo.currentAP;
        meleeStrength = enemyInfo.meleeStrength;

        armorStats = enemyInfo.armorStats;
    }
}
