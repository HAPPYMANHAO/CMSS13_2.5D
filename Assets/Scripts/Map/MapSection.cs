using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 地图区域 - 挂在每个地图Grid上
/// </summary>
public class MapSection : MonoBehaviour
{
    [Header("地图信息")]
    [SerializeField] private string mapId;
    [SerializeField] private string mapName;
    [SerializeField] private bool isStartingMap = false;
    
    [Header("地图层级")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    
    [Header("生成点")]
    [SerializeField] private List<MapSpawnPoint> spawnPoints = new List<MapSpawnPoint>();
    
    [Header("环境")]
    [SerializeField] private List<GameObject> decorations = new List<GameObject>();
    [SerializeField] private List<MapTrigger> triggers = new List<MapTrigger>();
    
    // 运行时数据
    public string MapId => mapId;
    public string MapName => mapName;
    public bool IsStartingMap => isStartingMap;
    
    private void Awake()
    {
        // 如果没有设置MapId，使用GameObject名字
        if (string.IsNullOrEmpty(mapId))
        {
            mapId = gameObject.name;
        }
        
        // 自动查找组件
        AutoFindComponents();
    }
    
    private void Start()
    {
        // 注册到MapManager
        MapManager.Instance?.RegisterMapSection(this);
        
        // 如果是起始地图，设置为主地图
        if (isStartingMap && MapManager.Instance != null)
        {
            MapManager.Instance.SetCurrentMap(mapId);
        }
    }
    
    private void OnDestroy()
    {
        MapManager.Instance?.UnregisterMapSection(this);
    }
    
    /// <summary>
    /// 自动查找地图组件
    /// </summary>
    private void AutoFindComponents()
    {
        // 在子物体中查找Tilemap
        if (groundTilemap == null)
            groundTilemap = transform.Find("Ground")?.GetComponent<Tilemap>();
        if (wallTilemap == null)
            wallTilemap = transform.Find("Walls")?.GetComponent<Tilemap>();
        if (decorationTilemap == null)
            decorationTilemap = transform.Find("Decorations")?.GetComponent<Tilemap>();
        
        // 查找所有生成点
        if (spawnPoints.Count == 0)
        {
            spawnPoints.AddRange(GetComponentsInChildren<MapSpawnPoint>(true));
        }
        
        // 查找所有触发器
        if (triggers.Count == 0)
        {
            triggers.AddRange(GetComponentsInChildren<MapTrigger>(true));
        }
    }
    
    /// <summary>
    /// 获取指定ID的生成点位置
    /// </summary>
    public Vector3? GetSpawnPointPosition(string spawnPointId)
    {
        if (string.IsNullOrEmpty(spawnPointId))
        {
            // 返回第一个生成点
            if (spawnPoints.Count > 0)
                return spawnPoints[0].transform.position;
            return null;
        }
        
        var spawnPoint = spawnPoints.Find(sp => sp.SpawnPointId == spawnPointId);
        if (spawnPoint != null)
            return spawnPoint.transform.position;
            
        // 如果没找到，返回第一个
        if (spawnPoints.Count > 0)
            return spawnPoints[0].transform.position;
            
        return null;
    }
    
    /// <summary>
    /// 激活地图
    /// </summary>
    public void Activate()
    {
        gameObject.SetActive(true);
        
        // 激活所有子物体
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
        // 触发地图激活事件
        OnMapActivated?.Invoke(this);
    }
    
    /// <summary>
    /// 禁用地图
    /// </summary>
    public void Deactivate()
    {
        // 触发地图禁用事件
        OnMapDeactivated?.Invoke(this);
        
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 检查某位置是否可行走
    /// </summary>
    public bool IsWalkable(Vector3 worldPosition)
    {
        if (wallTilemap == null) return true;
        
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPosition);
        return !wallTilemap.HasTile(cellPosition);
    }
    
    /// <summary>
    /// 获取地图边界
    /// </summary>
    public Bounds GetMapBounds()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        
        // 合并所有Tilemap的边界
        if (groundTilemap != null)
            bounds.Encapsulate(groundTilemap.localBounds);
        if (wallTilemap != null)
            bounds.Encapsulate(wallTilemap.localBounds);
        if (decorationTilemap != null)
            bounds.Encapsulate(decorationTilemap.localBounds);
            
        return bounds;
    }
    
    #region Events
    
    public event System.Action<MapSection> OnMapActivated;
    public event System.Action<MapSection> OnMapDeactivated;
    
    #endregion
    
    #region Gizmos
    
    private void OnDrawGizmosSelected()
    {
        // 绘制地图边界
        Gizmos.color = Color.cyan;
        Bounds bounds = GetMapBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        // 绘制生成点
        Gizmos.color = Color.green;
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawSphere(spawnPoint.transform.position, 0.3f);
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(spawnPoint.transform.position + Vector3.up * 0.5f, spawnPoint.SpawnPointId);
                #endif
            }
        }
    }
    
    #endregion
}