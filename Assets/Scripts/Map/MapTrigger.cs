using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 通用地图触发器 - 可以触发各种地图事件
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Enter,      // 进入时触发
        Exit,       // 退出时触发
        Stay        // 停留时持续触发
    }
    
    [Header("触发设置")]
    [SerializeField] private TriggerType triggerType = TriggerType.Enter;
    [SerializeField] private bool requirePlayerTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool onlyTriggerOnce = false;
    [SerializeField] private float cooldownTime = 0f; // 冷却时间
    
    [Header("事件")]
    public UnityEvent<GameObject> OnTrigger;
    public UnityEvent OnTriggerNoParam;
    
    private bool hasTriggered = false;
    private float lastTriggerTime = -999f;
    private Collider triggerCollider;
    
    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (triggerType != TriggerType.Enter) return;
        TryTrigger(other.gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (triggerType != TriggerType.Exit) return;
        TryTrigger(other.gameObject);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (triggerType != TriggerType.Stay) return;
        TryTrigger(other.gameObject);
    }
    
    private void TryTrigger(GameObject other)
    {
        // 检查标签
        if (requirePlayerTag && !other.CompareTag(playerTag))
            return;
        
        // 检查是否只触发一次
        if (onlyTriggerOnce && hasTriggered)
            return;
        
        // 检查冷却时间
        if (Time.time - lastTriggerTime < cooldownTime)
            return;
        
        // 触发事件
        ExecuteTrigger(other);
    }
    
    private void ExecuteTrigger(GameObject other)
    {
        hasTriggered = true;
        lastTriggerTime = Time.time;
        
        OnTrigger?.Invoke(other);
        OnTriggerNoParam?.Invoke();
        
        Debug.Log($"[MapTrigger] {gameObject.name} 被触发");
    }
    
    /// <summary>
    /// 手动触发
    /// </summary>
    public void ManualTrigger(GameObject target = null)
    {
        ExecuteTrigger(target);
    }
    
    /// <summary>
    /// 重置触发器
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        lastTriggerTime = -999f;
    }
    
    private void OnDrawGizmos()
    {
        Color gizmoColor = triggerType switch
        {
            TriggerType.Enter => new Color(0, 1, 0, 0.3f),
            TriggerType.Exit => new Color(1, 0, 0, 0.3f),
            TriggerType.Stay => new Color(1, 1, 0, 0.3f),
            _ => Color.gray
        };
        
        Gizmos.color = gizmoColor;
        
        if (TryGetComponent<BoxCollider>(out var boxCollider))
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
        else if (TryGetComponent<SphereCollider>(out var sphereCollider))
        {
            Gizmos.DrawSphere(transform.position + sphereCollider.center, sphereCollider.radius);
        }
    }
}