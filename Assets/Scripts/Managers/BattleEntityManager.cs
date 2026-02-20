using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleEntityManager : MonoBehaviour
{
    [Header("SpawnPoints")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    public List<BattleEntityBase> allBattleEntities = new List<BattleEntityBase>();
    public IEnumerable<PartyBattleEntity> partyEntities =>
        allBattleEntities.OfType<PartyBattleEntity>();
    public IEnumerable<EnemyBattleEntity> enemyEntities =>
        allBattleEntities.OfType<EnemyBattleEntity>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    public BattleTurnManager turnManager;

    private BattleVisualGUI battleVisualGUI;

    public PartyBattleEntity currentPlayerEntity;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
        turnManager = FindFirstObjectByType<BattleTurnManager>();

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

            partyBattleEntity.entityFaction = Faction.Survivor;
            partyBattleEntity.OnEntityDeath += HandleEntityDead;

            CharacterBattleVisual tempBattleVisual =
                Instantiate(currentParty[i].memberBattleVisualPerfab, partySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = partyBattleEntity;
            partyBattleEntity.battleVisual = tempBattleVisual;

            battleVisualGUI.BindHealthBar(partyBattleEntity);

            allBattleEntities.Add(partyBattleEntity);
        }

        currentPlayerEntity = partyEntities.First();
        battleVisualGUI.SelectLeftHand();
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
            enemyBattleEntity.OnEntityDeath += HandleEntityDead;

            CharacterBattleVisual tempBattleVisual =
                Instantiate(currentEnemy[i].enemyBattleVisualPerfab, enemySpawnPoints[i].position, Quaternion.identity)
                .GetComponent<CharacterBattleVisual>();
            tempBattleVisual.battleEntity = enemyBattleEntity;
            enemyBattleEntity.battleVisual = tempBattleVisual;

            tempBattleVisual.GetComponentInChildren<EnemyHealthBarGUI>().EnemyHealthBarBind(enemyBattleEntity);

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
        if (currentPlayerEntity == null || currentPlayerEntity.EntityIsDead())
        {
            return;
        }

        ItemInstance handItem = currentPlayerEntity.GetCurrentActiveHandItem();
        if (handItem == null || handItem.itemData == null) return;

        ActionBase currentAction = handItem.GetCurrentAction();
        if (currentAction == null || string.IsNullOrEmpty(currentAction.actionName)) return;

        BattleEntityBase[] targetEntities = { entity };
        currentPlayerEntity.ExecuteAction(currentAction, targetEntities);

        if (handItem is StackableItemInstance)
        {
            StackableItemInstance stackableItemInstance = (StackableItemInstance)handItem;
            if (stackableItemInstance.IsEmpty)
            {
                currentPlayerEntity.SentHoldItemToInventory();
            }
        }

        battleVisualGUI.UpdateHandVisuals();

        battleVisualGUI.isPlayerCanExecuteAction = false;
        battleVisualGUI.playerActionDelayTimer = currentAction.actionDelay;
    }
    //----------------------------Event----------------------------//
    private void HandleEntityDead(BattleEntityBase deadEntity)
    {
        if (allBattleEntities.Contains(deadEntity))
        {
            if(deadEntity is PartyBattleEntity)
            {
                SavePartyStats();
            }
            allBattleEntities.Remove(deadEntity);
        }

        //turnManager.logGUI.UpdateLog();
        turnManager.CheckBattleVictoryOrDefeat();
    }
    //----------------------------Save Stats----------------------------//
    public void SavePartyStats()
    {
        foreach (PartyBattleEntity member in partyEntities)
        {
            partyManager.SaveMemberStatsAfterBattle(member);
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
    public int currentLevel;

    public Dictionary<SkillType, int> skills;

    public bool isAutoExecuteAction = false;

    public HandSlot leftHandEquipment = new HandSlot();
    public HandSlot rightHandEquipment = new HandSlot();
    public EquipmentSlot armorEquipment = new EquipmentSlot();

    public enum EntityHandsSlot { Left, Right }

    public EntityHandsSlot currentActiveHand = EntityHandsSlot.Left;

    public ItemInstance GetCurrentActiveHandItem()
    {
        return currentActiveHand == EntityHandsSlot.Left ? leftHandEquipment?.item : rightHandEquipment?.item;
    }
    public ItemInstance SentHoldItemToInventory()
    {
        ItemInstance itemToReturn = GetCurrentActiveHandItem();

        if (currentActiveHand == EntityHandsSlot.Left)
        {
            leftHandEquipment.item = null;
        }
        else if (currentActiveHand == EntityHandsSlot.Right)
        {
            rightHandEquipment.item = null;
        }

        return itemToReturn;
    }

    public void GetItemFromToInventory(ItemInstance item)
    {
        if (currentActiveHand == EntityHandsSlot.Left)
        {
            leftHandEquipment.item = item;
            battleVisual.ActiveItemDisplyer(item.itemData.icon);
        }
        else if (currentActiveHand == EntityHandsSlot.Right)
        {
            rightHandEquipment.item = item;
            battleVisual.ActiveItemDisplyer(item.itemData.icon);
        }
    }

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
        damageResistanceStats = memberInfo.damageResistanceStats;

        entityAI = memberInfo.entityAI;
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
    public int rangedStrength;
    public float armorPenetration;

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
        rangedStrength = enemyInfo.rangedStrength;
        armorPenetration = enemyInfo.armorPenetration;

        armorStats = enemyInfo.armorStats;
        damageResistanceStats = enemyInfo.damageResistanceStats;

        entityAI = enemyInfo.entityAI;
    }
}
