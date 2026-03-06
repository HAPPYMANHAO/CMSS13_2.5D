using Unity.VisualScripting;
using UnityEngine;

public class GunInstance : ItemInstance
{
    public GunItemBase GunData => itemData as GunItemBase;

    public int currentAmmoCount { get; private set; }
    public AmmoBase loadedAmmoType { get; private set; } 

    // 当前射击模式
    public FireMode currentFireMode => GunData.availableFireModes[_fireModeIndex];
    private int _fireModeIndex = 0;

    public bool IsEmpty => currentAmmoCount <= 0;
    public bool IsFull => currentAmmoCount >= GunData.magazineCapacity;

    public GunInstance(GunItemBase data) : base(data) { }

    // 获取当前弹药的 ProjectileInfo（供 DamageCalculator.cs 使用）
    public ProjectileInfo GetCurrentProjectileInfo()
        => loadedAmmoType?.projectileInfo;

    // 装弹：从一个 StackableItemInstance（弹药）中填充弹夹
    // 返回实际装入的数量
    public int Reload(StackableItemInstance ammoStack)
    {
        if (ammoStack.itemData is not AmmoBase newAmmo) return 0;
        if (newAmmo.ammoType != GunData.acceptedAmmoType) return 0;

        if (loadedAmmoType != null && loadedAmmoType != newAmmo && currentAmmoCount > 0)
        {
            InventoryManager.instance.AddItemFromData(loadedAmmoType, currentAmmoCount);
            currentAmmoCount = 0;
        }

        loadedAmmoType = newAmmo;

        int canLoad = GunData.magazineCapacity - currentAmmoCount;
        int toLoad = Mathf.Min(canLoad, ammoStack.currentQuantity);

        currentAmmoCount += toLoad;
        ammoStack.currentQuantity -= toLoad;

        return toLoad;
    }

    // 射击消耗一发
    public bool ConsumeAmmo()
    {
        if (IsEmpty) return false;
        currentAmmoCount--;
        Debug.Log(currentAmmoCount);
        return true;
    }

    // 切换射击模式（循环）
    public void CycleFireMode()
    {
        _fireModeIndex = (_fireModeIndex + 1) % GunData.availableFireModes.Count;
    }
}
