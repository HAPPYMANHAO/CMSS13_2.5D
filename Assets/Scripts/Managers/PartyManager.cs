using System.Collections.Generic;
using UnityEngine;
using static PartyMemberInfo;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allPartyMember;
    [SerializeField] private List<CurrentPartyMemberInfo> currentPartyMember;

    [SerializeField] private PartyMemberInfo defaultMember;//主角Rermadon，有时候也可以称为player The protagonist Rermadon,Sometimes can also be called "player"

    public void Awake()
    {
        AddPartyMemberByName(defaultMember.memberName);
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
    public GameObject memberBattleVisualPerfab;
    public GameObject memberOverworldVisualPerfab;

    public Dictionary<SkillType, int> skills;
    public Dictionary<DamageType, ArmorStats> armorStats;

    public EntityAI entityAI;

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

        this.entityAI = partyMember.entityAI;
    }
}
