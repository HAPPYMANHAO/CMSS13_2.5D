using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static RangedAction;

public class ProjectileInstance : MonoBehaviour
{
    private SpriteRenderer projectileSprite;
    private ProjectileInfo projectileConfig;
    private Light projectileSpriteGlows;
    private Vector3 targetPos;
    private bool isFlying = false;

    private BattleEntityBase user; // user
    private BattleEntityBase targetEntity; // trget
    private RangedAction actionSource; // action

    private void Awake()
    {
        projectileSprite = GetComponent<SpriteRenderer>();
        projectileSpriteGlows = GetComponent<Light>();
    }

    public void ProjectileSetup(BattleEntityBase user, BattleEntityBase target, RangedAction action)
    {
        projectileConfig = action.projectileInfo;
        this.user = user;
        this.targetEntity = target;
        this.actionSource = action;

        var collider = target.battleVisual.GetComponentInChildren<BoxCollider>();
        if (collider != null)
        {
            targetPos = collider.bounds.center;
        }
        else
        {
            targetPos = target.battleVisual.transform.position + Vector3.up;
        }

        projectileSprite.sprite = action.projectileInfo.projectileSprite;
        projectileSpriteGlows.color = action.projectileInfo.projectileGlowsColor;
        ChangeDirection();
        isFlying = true;
    }

    private void Update()
    {
        ChangeDirection();
        if (!isFlying) return;

        if (ReachedTarget())
        {
            OnHit();
        }
    }

    private void ChangeDirection()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, projectileConfig.projectileSpeed * Time.deltaTime);

        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            //将向上的精灵修正为朝向目标 Change upward sprite to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }


    private bool ReachedTarget()
    {
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            return true;
        }
        else return false;
    }

    private void OnHit()
    {
        isFlying = false;
        int damage = targetEntity.EntityTakeDamage(
            user, projectileConfig.projectileDamageType, actionSource
        );

        UpdateActionLog(user, targetEntity , damage);

        Destroy(gameObject);
    }

    public void UpdateActionLog(BattleEntityBase userEntity, BattleEntityBase target, int amount)
    {
        var handleLog = actionSource.actionLogTemplate.GetLocalizedStringAsync(new
        {
            user = userEntity.memberName,
            target = target.memberName,
            damage = amount.ToString()
        });
        handleLog.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                string finalLog = op.Result;
                ActionBase.OnActionLogged?.Invoke(finalLog);
            }
        };
    }
}
