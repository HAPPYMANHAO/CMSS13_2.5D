using UnityEngine;

/// <summary>
/// 地图生成点 - 玩家进入地图时的初始位置
/// </summary>
public class MapSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnPointId;
    [SerializeField] private bool isDefaultSpawnPoint = false;
    [SerializeField] private Vector3 facingDirection = Vector3.back; // 默认朝向
    
    public string SpawnPointId => string.IsNullOrEmpty(spawnPointId) ? gameObject.name : spawnPointId;
    public bool IsDefaultSpawnPoint => isDefaultSpawnPoint;
    public Vector3 FacingDirection => facingDirection;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        
        // 绘制朝向
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, facingDirection * 0.5f);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.3f, SpawnPointId);
        #endif
    }
}