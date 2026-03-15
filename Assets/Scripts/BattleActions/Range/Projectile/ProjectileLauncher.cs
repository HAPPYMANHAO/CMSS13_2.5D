using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // 投射物预制体，这只是模板

    public void ProjectileLaunch(BattleEntityBase user, BattleEntityBase target, ActionBase action)
    {
        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        ProjectileInstance projectile = go.GetComponent<ProjectileInstance>();

        projectile.ProjectileSetup(user, target, action);
    }

    public void ProjectileLaunch(BattleEntityBase user, BattleEntityBase target, ActionBase action, ProjectileInfo projectile, float projectileAccuracy)
    {
        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        ProjectileInstance projectileInstance = go.GetComponent<ProjectileInstance>();

        projectileInstance.ProjectileSetup(user, target, action, projectile, projectileAccuracy);
    }
}
