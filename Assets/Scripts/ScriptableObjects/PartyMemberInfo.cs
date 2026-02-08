using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PartyMemberInfo;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string memberName;
    public int startLevel;
    public int baseHealth;
    public int MaxAP;
    public int eachTurnRecoveredAP;
    public int healthCRIT = 0;
    public int healthCRITShock = -50;
    public int healthDead = -100;
    public GameObject MemberBattleVisualPerfab;
    public GameObject MemberOverworldVisualPerfab;

    public List<InitialSkill> startingSkills = new List<InitialSkill>();
    public List<ArmorStats> armorStats = new List<ArmorStats>();

    public EntityAI entityAI;

    private bool skillsInitialized = false;
    private bool armorInitialized= false;

    public Dictionary<SkillType, int> GetSkillDictionary()
    {
        Dictionary<SkillType, int> dict = new Dictionary<SkillType, int>();
        foreach (var skill in startingSkills)
        {
            dict[skill.skillType] = skill.level;
        }
        return dict;
    }

    public Dictionary<DamageType, ArmorStats> GetArmorDictionary()
    {
        Dictionary<DamageType, ArmorStats> dict = new Dictionary<DamageType, ArmorStats>();
        foreach (var armor in armorStats)
        {
            dict[armor.armorType] = armor;
        }
        return dict;
    }

    //----------------------OnValidate----------------------//

    private void OnValidate()
    {
        SkillType[] allSkills = (SkillType[])System.Enum.GetValues(typeof(SkillType));

        if (startingSkills.Count != allSkills.Length)
        {
            Dictionary<SkillType, int> existingLevels = new Dictionary<SkillType, int>();
            foreach (var _skill in startingSkills)
            {
                existingLevels[_skill.skillType] = _skill.level;
            }

            startingSkills.Clear();
        
            foreach (var _skill in allSkills)
            {
                startingSkills.Add(new InitialSkill
                {
                    skillType = _skill,
                    level = existingLevels.ContainsKey(_skill) ? existingLevels[_skill] : 0
                });
            }
        }

        if (!skillsInitialized)
        {
            InitializeSkills();
            skillsInitialized = true;
        }
        if (!armorInitialized)
        {
            InitializeAromr();
            armorInitialized = true;
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

            foreach (var _aromr in armorStats)
            {
                armorStats.Add(new ArmorStats
                {
                    armorType = _aromr.armorType,
                    armorIntegrity = existingArmorStrength.ContainsKey(_aromr.armorType) ? existingArmorStrength[_aromr.armorType] : 0f,
                    armorValue = existingAromrValue.ContainsKey(_aromr.armorType) ? existingAromrValue[_aromr.armorType] : 0,
                });
            }
        }       
    }

    private void InitializeSkills()
    {
        startingSkills.Clear();

        SkillType[] allSkills = (SkillType[])System.Enum.GetValues(typeof(SkillType));

        foreach (var skill in allSkills)
        {
            startingSkills.Add(new InitialSkill
            {
                skillType = skill,
                level = 0
            });
        }
    }

    private void InitializeAromr()
    {
        armorStats.Clear();

        DamageType[] allDamgaeType = (DamageType[])System.Enum.GetValues(typeof(DamageType));

        foreach(var damageType in allDamgaeType)
        {
            armorStats.Add(new ArmorStats
            {
                armorType = damageType,
                armorIntegrity = 0,
                armorValue = 0,
            });
        }
    }
}



