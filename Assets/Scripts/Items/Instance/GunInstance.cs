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

    // 获取当前弹药的 ProjectileInfo（供 gunFireAction 使用）
    public ProjectileInfo GetCurrentProjectileInfo()
        => loadedAmmoType?.projectileInfo;

    // 装弹：从一个 StackableItemInstance（弹药）中填充弹夹
    // 返回实际装入的数量
    public int Reload(StackableItemInstance ammoStack)
    {
        if (ammoStack.itemData is not AmmoBase ammo) return 0;
        if (ammo.ammoType != GunData.acceptedAmmoType) return 0;

        int canLoad = GunData.magazineCapacity - currentAmmoCount;
        int toLoad = Mathf.Min(canLoad, ammoStack.currentQuantity);

        loadedAmmoType = ammo;
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
