using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PartyMemberInfo;
using static UnityEngine.EventSystems.EventTrigger;

public class PartyManager : MonoBehaviour
{
    private OverworldVisualGUI overworldVisual;

    [SerializeField] private PartyMemberInfo[] allPartyMember;
    [SerializeField] private List<CurrentPartyMemberInfo> currentPartyMember;

    [SerializeField] private PartyMemberInfo defaultMember;//主角Rermadon，有时候也可以称为player The protagonist Rermadon,Sometimes can also be called "player"

    private Vector3 playerPosition;

    public static PartyManager instance;//self

    public static event Action OnPartyMemberUpdated;

    public CurrentPartyMemberInfo currentPlayerEntity;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        AddPartyMemberByName(defaultMember.memberName);
        currentPlayerEntity = currentPartyMember.FirstOrDefault();
    }

    private void Start()
    {
        overworldVisual = FindFirstObjectByType<OverworldVisualGUI>();
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

    public static void PartyUpdated()
    {
        OnPartyMemberUpdated?.Invoke();
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
        OnPartyMemberUpdated?.Invoke();
        member.isDead = partyBattleEntity.EntityIsDead();
        overworldVisual.UpdateHandVisuals();
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
public class CurrentPartyMemberInfo : IHandsOwner
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

    public HandSlot leftHandEquipment { get; set; } = new HandSlot();
    public HandSlot rightHandEquipment { get; set; } = new HandSlot();
    public EntityHandsSlot currentActiveHand { get; set; } = EntityHandsSlot.Left;

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

    // ── 装备栏 Equipment Slots ──────────────────────
    // 防护槽
    public Dictionary<EquipmentSlotType, ItemInstance> protectionSlots
        = new Dictionary<EquipmentSlotType, ItemInstance>();

    // 存储槽（背包/腰带/口袋）
    public Dictionary<EquipmentSlotType, StorageItemInstance> storageSlots
        = new Dictionary<EquipmentSlotType, StorageItemInstance>();

    // ── 装备操作 ────────────────────────────────────
    public bool TryEquip(ItemInstance item)
    {
        // 判断是防护装备还是存储装备
        if (item.itemData is ArmorItemBase armor)
        {
            return TryEquipArmor(item, armor.slotType);
        }
        if (item.itemData is StorageItemBase storage)
        {
            return TryEquipStorage(item as StorageItemInstance, storage.slotType);
        }
        return false;
    }

    private bool TryEquipArmor(ItemInstance item, EquipmentSlotType slot)
    {
        // 如果槽位已有装备，先卸下
        if (protectionSlots.TryGetValue(slot, out var existing) && existing != null)
        {
            return false;
        }

        protectionSlots[slot] = item;
        (item.itemData as ArmorItemBase)?.OnEquip(this);
        return true;
    }

    private bool TryEquipStorage(StorageItemInstance item, EquipmentSlotType slot)
    {
        if (storageSlots.TryGetValue(slot, out var existing) && existing != null)
        {
            return false;
        }

        storageSlots[slot] = item;
        return true;
    }

    public ItemInstance UnequipSlot(EquipmentSlotType slot)
    {
        if (protectionSlots.TryGetValue(slot, out var item) && item != null)
        {
            (item.itemData as ArmorItemBase)?.OnUnEquip(this);
            protectionSlots[slot] = null;
            return item;
        }
        if (storageSlots.TryGetValue(slot, out var storage) && storage != null)
        {
            foreach (var storageItem in storage.StoredItems)
            {
                InventoryManager.instance.AddItem(storageItem);
            }

            storage.RemoveAllItem();
            storageSlots[slot] = null;
            return storage;
        }
        return null;
    }

    public ItemInstance GetEquipped(EquipmentSlotType slot)
    {
        if (protectionSlots.TryGetValue(slot, out var item)) return item;
        if (storageSlots.TryGetValue(slot, out var storage)) return storage;
        return null;
    }


    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);

        if (currentHealth < healthDead)
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
