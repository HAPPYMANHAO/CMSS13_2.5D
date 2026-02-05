using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private SpriteRenderer projectileSprite;
    private ProjectileInfo projectileConfig;
    private Light projectileSpriteGlows;
    private Vector3 targetPos;
    private bool isFlying = false;

    private void Awake()
    {
        projectileSprite = GetComponent<SpriteRenderer>();
        projectileSpriteGlows = GetComponent<Light>();
    }

    public void ProjectileSetup(ProjectileInfo info, Vector3 target)
    {
        projectileConfig = info;
        targetPos = target;
        projectileSprite.sprite = info.projectileSprite;
        projectileSpriteGlows.color = info.projectileGlowsColor;

        isFlying = true;
    }

    private void Update()
    {
        if (!isFlying) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, projectileConfig.projectileSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            isFlying = false;
            //造成伤害！
            Destroy(gameObject);
        }
    }
}
