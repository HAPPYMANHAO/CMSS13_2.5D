using UnityEngine;

[CreateAssetMenu(menuName = "Item/Ammo")]
public class AmmoBase : ConsumableItemBase
{
    public AmmoType ammoType;         // 例如 9mm, 5_56, 12Gauge
    public ProjectileInfo projectileInfo; // 弹头的投射物数据
    public string caliberName;        // 显示用
}
public enum AmmoType { Ammo_9mm, Ammo_10x24mm_caseless}

