using UnityEngine;

[CreateAssetMenu(menuName = "New Projectile")]
public class ProjectileInfo : ScriptableObject
{
    public Sprite projectileSprite;
    public Color projectileGlowsColor;

    public DamageType projectileDamageType;
    public int projectileDamage;
    public int projectileArmorPenetration;
    public float projectileSpeed;
}
