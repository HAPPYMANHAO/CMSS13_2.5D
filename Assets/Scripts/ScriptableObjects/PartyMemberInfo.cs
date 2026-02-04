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
    }

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

    [SerializeField, HideInInspector]
    private bool skillsInitialized = false;

    public void InitializeSkills()
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
}

[System.Serializable]
public struct InitialSkill
{
    public SkillType skillType;
    public int level;
}


