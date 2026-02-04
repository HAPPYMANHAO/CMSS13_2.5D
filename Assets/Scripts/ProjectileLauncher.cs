using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    
    [SerializeField] private GameObject projectilePrefab; // 投射物预制体

    public void ProjectileLaunch(ProjectileInfo info, Vector3 target)
    {       
        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);           
        ProjectileInstance projectile = go.GetComponent<ProjectileInstance>();

        projectile.ProjectileSetup(info, target);
    }
}
