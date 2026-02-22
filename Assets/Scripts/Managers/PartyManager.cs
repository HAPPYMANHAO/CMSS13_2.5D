using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PartyMemberInfo;
using static UnityEngine.EventSystems.EventTrigger;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private OverworldVisualGUI overworldVisual;

    [SerializeField] private PartyMemberInfo[] allPartyMember;
    [SerializeField] private List<CurrentPartyMemberInfo> currentPartyMember;

    [SerializeField] private PartyMemberInfo defaultMember;//主角Rermadon，有时候也可以称为player The protagonist Rermadon,Sometimes can also be called "player"

    private Vector3 playerPosition;

    private static GameObject instance;//self

    public CurrentPartyMemberInfo currentPlayerEntity;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return; 
        }
        instance = this.gameObject;

        DontDestroyOnLoad(gameObject);
        AddPartyMemberByName(defaultMember.memberName);
        currentPlayerEntity = currentPartyMember.FirstOrDefault();
    }
    public void AddPartyMemberByName(string memberName)
    {
        for (int i = 0; i < allPartyMember.Length; i++)
        {
            if (memberName == allPartyMember[i].memberName)
            {
                CurrentPartyMemberInfo newPartyMember = new CurrentPartyMemberInfo(allPartyMember[i]);

                currentPartyMember.Add(newPartyMember);
            }
        }
    }

    public List<CurrentPartyMemberInfo> GetCurrentPartyMember()
    {
        return currentPartyMember;
    }

    public void SaveMemberStatsAfterBattle(PartyBattleEntity partyBattleEntity)
    {
        var member = currentPartyMember.FirstOrDefault(m => m.memberName == partyBattleEntity.memberName);
        if (member == null) return;
        member.currentHealth = partyBattleEntity.currentHealth;
        member.leftHandEquipment.item = partyBattleEntity.leftHandEquipment.item;   // 回写手部
        member.rightHandEquipment.item = partyBattleEntity.rightHandEquipment.item;
        member.isDead = partyBattleEntity.EntityIsDead();
    }

    public void SetPlayerPosition(Vector3 newPosition)
    {
        playerPosition = newPosition;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }
}

[System.Serializable]
public class CurrentPartyMemberInfo
{
    public string memberName;
    public int currentLevel;
    public int currentHealth;
    public int maxHealth;
    public int maxAP;
    public int eachTurnRecoveredAP;
    public int currentAP;
    public int currentEXP;
    public int maxEXP;
    public int healthCRIT;
    public int healthCRITShock;
    public int healthDead;

    public bool isDead = false;

    public GameObject memberBattleVisualPerfab;
    public GameObject memberOverworldVisualPerfab;

    public Dictionary<SkillType, int> skills;
    public Dictionary<DamageType, ArmorStats> armorStats;
    public Dictionary<DamageType, float> damageResistanceStats;

    public EntityAI entityAI;

    public HandSlot leftHandEquipment = new HandSlot();
    public HandSlot rightHandEquipment = new HandSlot();
    public EntityHandsSlot currentActiveHand = EntityHandsSlot.Left;

    public event Action OnHealthChanged;

    public ItemInstance GetCurrentActiveHandItem()
    {
        return currentActiveHand == EntityHandsSlot.Left ? leftHandEquipment?.item : rightHandEquipment?.item;
    }

    public void GetItemFromToInventory(ItemInstance item)
    {
        if (currentActiveHand == EntityHandsSlot.Left)
        {
            leftHandEquipment.item = item;
            //overworldVisual.ActiveItemDisplyer(item.itemData.icon);
        }
        else if (currentActiveHand == EntityHandsSlot.Right)
        {
            rightHandEquipment.item = item;
            //overworldVisual.ActiveItemDisplyer(item.itemData.icon);
        }
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

    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);

        if(currentHealth < healthDead)
        {
            isDead = true;
        }
    }

    private void SetHealth(int newHealth)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Clamp(newHealth, healthDead, maxHealth);
        OnHealthChanged?.Invoke();   
    }

    public void RevoverHealth(int recoverHealth)
    {
        SetHealth(currentHealth + recoverHealth);
    }

    public CurrentPartyMemberInfo(PartyMemberInfo partyMember)
    {
        this.memberName = partyMember.memberName;
        this.currentLevel = partyMember.startLevel;
        this.maxHealth = partyMember.baseHealth;
        this.currentHealth = this.maxHealth;
        this.maxAP = partyMember.MaxAP;
        this.eachTurnRecoveredAP = partyMember.eachTurnRecoveredAP;
        this.currentAP = (maxAP / 2);
        this.healthCRIT = partyMember.healthCRIT;
        this.healthCRITShock = partyMember.healthCRITShock;
        this.healthDead = partyMember.healthDead;
        this.memberBattleVisualPerfab = partyMember.MemberBattleVisualPerfab;
        this.memberOverworldVisualPerfab = partyMember.MemberOverworldVisualPerfab;

        // 克隆数据为字典 cloned data in the form of a dictionary
        skills = partyMember.GetSkillDictionary();
        armorStats = partyMember.GetArmorDictionary();
        damageResistanceStats = partyMember.GetDamageResistanceDictionary();

        this.entityAI = partyMember.entityAI;
    }
}
