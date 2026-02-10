using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{  
    [SerializeField] private GameObject projectilePrefab; // 投射物预制体，这只是模板

    public void ProjectileLaunch(BattleEntityBase user, BattleEntityBase target, RangedAction action)
    {       
        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);           
        ProjectileInstance projectile = go.GetComponent<ProjectileInstance>();

        projectile.ProjectileSetup(user, target ,action);
    }
}
