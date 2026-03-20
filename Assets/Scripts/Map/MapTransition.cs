using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 地图传送点 - 触发地图切换
/// </summary>
[RequireComponent(typeof(Collider))]
public class MapTransition : MonoBehaviour
{
    [Header("传送设置")]
    [SerializeField] private string targetMapId;
    [SerializeField] private string targetSpawnPointId;
    
    [Header("传送条件")]
    [SerializeField] private bool requirePlayerTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool onlyTriggerOnce = false;
    
    [Header("视觉效果")]
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [Header("事件")]
    public UnityEvent OnTransitionStart;
    public UnityEvent OnTransitionComplete;
    
    private bool hasTriggered = false;
    private Collider triggerCollider;
    
    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        
        // 确保有ID
        if (string.IsNullOrEmpty(targetMapId))
        {
            Debug.LogWarning($"[MapTransition] {gameObject.name} 没有设置目标地图ID！");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 检查是否只触发一次
        if (onlyTriggerOnce && hasTriggered)
            return;
        
        // 检查标签
        if (requirePlayerTag && !other.CompareTag(playerTag))
            return;
        
        // 触发传送
        TriggerTransition(other.gameObject);
    }
    
    /// <summary>
    /// 触发地图传送
    /// </summary>
    public void TriggerTransition(GameObject playerObject)
    {
        if (string.IsNullOrEmpty(targetMapId))
        {
            Debug.LogError($"[MapTransition] {gameObject.name} 目标地图ID为空！");
            return;
        }
        
        hasTriggered = true;
        
        Debug.Log($"[MapTransition] 传送到地图: {targetMapId}, 生成点: {targetSpawnPointId}");
        
        // 触发事件
        OnTransitionStart?.Invoke();
        
        // 调用MapManager进行传送
        if (MapManager.Instance != null)
        {
            MapManager.Instance.TransitionToMap(targetMapId, targetSpawnPointId, useFadeEffect, fadeDuration);
        }
        else
        {
            Debug.LogError("[MapTransition] MapManager.Instance 为 null！");
        }
        
        OnTransitionComplete?.Invoke();
    }
    
    /// <summary>
    /// 重置触发状态（用于可以重复使用的传送点）
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
    
    private void OnDrawGizmos()
    {
        // 绘制传送点图标
        Gizmos.color = new Color(0, 0.8f, 1f, 0.5f);
        
        if (TryGetComponent<BoxCollider>(out var boxCollider))
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (TryGetComponent<SphereCollider>(out var sphereCollider))
        {
            Gizmos.DrawSphere(transform.position + sphereCollider.center, sphereCollider.radius);
        }
        
        #if UNITY_EDITOR
        // 显示目标信息
        string label = $"To: {targetMapId}";
        if (!string.IsNullOrEmpty(targetSpawnPointId))
            label += $"\nSpawn: {targetSpawnPointId}";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f, label);
        
        // 绘制指向目标的箭头
        if (!string.IsNullOrEmpty(targetMapId))
        {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.ArrowHandleCap(
                0, 
                transform.position, 
                Quaternion.LookRotation(Vector3.up), 
                1f, 
                EventType.Repaint
            );
        }
        #endif
    }
}