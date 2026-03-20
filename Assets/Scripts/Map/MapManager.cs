using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

// 条件编译：只有在安装了Cinemachine包时才使用
#if CINEMACHINE_2 || CINEMACHINE_3
using Cinemachine;
#endif

/// <summary>
/// 地图管理器 - 管理所有地图区域和场景切换
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    [Header("地图设置")]
    [SerializeField] private List<MapSection> mapSections = new List<MapSection>();
    [SerializeField] private string startingMapId;
    [SerializeField] private string startingSpawnPointId;
    
    [Header("玩家引用")]
    [SerializeField] private GameObject playerPrefab;
    private GameObject currentPlayer;
    
    [Header("摄像机")]
    [SerializeField] private Transform mainCameraTransform; // 主摄像机Transform
    [SerializeField] private MonoBehaviour cinemachineCamera; // Cinemachine虚拟摄像机（可选）
    
    [Header("加载界面")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float minimumLoadingTime = 0.5f;
    
    private LoadingScreenGUI loadingScreen => LoadingScreenGUI.Instance;
    
    // 运行时数据
    private MapSection currentMap;
    private Dictionary<string, MapSection> mapDictionary = new Dictionary<string, MapSection>();
    private bool isTransitioning = false;
    
    // 玩家数据
    private PlayerMapData playerMapData = new PlayerMapData();
    
    #region Lifecycle
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 初始化加载界面
        // LoadingScreenGUI 是单例，会在自己的 Awake 中初始化
    }
    
    private void Start()
    {
        // 初始化地图
        InitializeMaps();
        
        // 生成玩家或定位现有玩家
        SetupPlayer();
    }
    
    #endregion
    
    #region Map Registration
    
    /// <summary>
    /// 注册地图区域（由MapSection调用）
    /// </summary>
    public void RegisterMapSection(MapSection section)
    {
        if (section == null || string.IsNullOrEmpty(section.MapId))
        {
            Debug.LogWarning("[MapManager] 尝试注册无效的地图区域！");
            return;
        }
        
        if (mapDictionary.ContainsKey(section.MapId))
        {
            Debug.LogWarning($"[MapManager] 地图ID '{section.MapId}' 已存在，将被覆盖！");
        }
        
        mapDictionary[section.MapId] = section;
        
        if (!mapSections.Contains(section))
        {
            mapSections.Add(section);
        }
        
        Debug.Log($"[MapManager] 注册地图: {section.MapId}");
    }
    
    /// <summary>
    /// 注销地图区域
    /// </summary>
    public void UnregisterMapSection(MapSection section)
    {
        if (section == null) return;
        
        if (mapDictionary.ContainsKey(section.MapId))
        {
            mapDictionary.Remove(section.MapId);
        }
        
        mapSections.Remove(section);
    }
    
    #endregion
    
    #region Map Transition
    
    /// <summary>
    /// 传送到指定地图
    /// </summary>
    public void TransitionToMap(string targetMapId, string spawnPointId = null, 
        bool useFade = true, float customFadeDuration = -1f)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[MapManager] 正在传送中，无法开始新的传送！");
            return;
        }
        
        if (!mapDictionary.ContainsKey(targetMapId))
        {
            Debug.LogError($"[MapManager] 目标地图 '{targetMapId}' 不存在！");
            return;
        }
        
        StartCoroutine(TransitionCoroutine(targetMapId, spawnPointId, useFade, customFadeDuration));
    }
    
    private IEnumerator TransitionCoroutine(string targetMapId, string spawnPointId, 
        bool useFade, float customFadeDuration)
    {
        isTransitioning = true;
        float startTime = Time.time;
        float fadeTime = customFadeDuration > 0 ? customFadeDuration : fadeDuration;
        
        // 检查加载界面
        if (useFade && loadingScreen == null)
        {
            Debug.LogWarning("[MapManager] LoadingScreenGUI 实例为 null！请确保场景中有 LoadingScreenGUI 对象。");
            useFade = false;
        }
        
        // 触发传送开始事件
        OnMapTransitionStart?.Invoke(currentMap?.MapId, targetMapId);
        
        // 1. 显示加载界面并等待淡入完成
        if (useFade && loadingScreen != null)
        {
            bool showComplete = false;
            loadingScreen.Show(fadeTime, () => showComplete = true);
            yield return new WaitUntil(() => showComplete);
        }
        
        // 更新进度: 20%
        loadingScreen?.UpdateProgress(0.2f);
        
        // 2. 禁用当前地图
        if (currentMap != null)
        {
            currentMap.Deactivate();
        }
        
        // 更新进度: 40%
        loadingScreen?.UpdateProgress(0.4f);
        yield return null;
        
        // 3. 激活目标地图
        MapSection targetMap = mapDictionary[targetMapId];
        targetMap.Activate();
        currentMap = targetMap;
        
        // 更新进度: 60%
        loadingScreen?.UpdateProgress(0.6f);
        
        // 4. 移动玩家到生成点
        if (currentPlayer != null)
        {
            Vector3? spawnPosition = targetMap.GetSpawnPointPosition(spawnPointId);
            if (spawnPosition.HasValue)
            {
                Vector3 previousPosition = currentPlayer.transform.position;
                currentPlayer.transform.position = spawnPosition.Value;
                
                // 立即更新摄像机位置，防止摄像机平滑移动
                UpdateCameraPosition(spawnPosition.Value, previousPosition);
                
                // 更新玩家朝向
                var spawnPoint = FindSpawnPoint(targetMapId, spawnPointId);
                if (spawnPoint != null)
                {
                    // 这里可以设置玩家朝向
                }
            }
            else
            {
                Debug.LogWarning($"[MapManager] 未找到生成点 '{spawnPointId}'，使用默认位置");
            }
        }
        
        // 更新进度: 80%
        loadingScreen?.UpdateProgress(0.8f);
        yield return null;
        
        // 5. 更新玩家数据
        playerMapData.currentMapId = targetMapId;
        playerMapData.currentSpawnPointId = spawnPointId;
        if (currentPlayer != null)
        {
            playerMapData.position = currentPlayer.transform.position;
        }
        
        // 更新进度: 90%
        loadingScreen?.UpdateProgress(0.9f);
        
        // 6. 确保最小加载时间
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadingTime)
        {
            float waitTime = minimumLoadingTime - elapsedTime;
            float startWait = Time.time;
            
            while (Time.time - startWait < waitTime)
            {
                float t = (Time.time - startWait) / waitTime;
                loadingScreen?.UpdateProgress(Mathf.Lerp(0.9f, 1f, t));
                yield return null;
            }
        }
        
        // 最终进度: 100%
        loadingScreen?.UpdateProgress(1f);
        
        // 7. 隐藏加载界面
        if (useFade && loadingScreen != null)
        {
            bool fadeComplete = false;
            loadingScreen.Hide(fadeTime, () => fadeComplete = true);
            yield return new WaitUntil(() => fadeComplete);
        }
        
        // 触发传送完成事件
        OnMapTransitionComplete?.Invoke(targetMapId);
        
        isTransitioning = false;
        
        Debug.Log($"[MapManager] 传送完成: {targetMapId}");
    }
    
    #endregion
    
    #region Player Management
    
    private void SetupPlayer()
    {
        // 尝试查找现有玩家
        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        
        // 如果没有找到，生成新玩家
        if (currentPlayer == null && playerPrefab != null)
        {
            currentPlayer = Instantiate(playerPrefab);
            currentPlayer.tag = "Player";
        }
        
        // 定位玩家到起始地图
        if (currentPlayer != null)
        {
            PositionPlayerAtStart();
        }
    }
    
    private void PositionPlayerAtStart()
    {
        string targetMapId = string.IsNullOrEmpty(startingMapId) ? null : startingMapId;
        string targetSpawnId = string.IsNullOrEmpty(startingSpawnPointId) ? null : startingSpawnPointId;
        
        // 如果没有设置起始地图，使用第一个可用的地图
        if (string.IsNullOrEmpty(targetMapId) && mapSections.Count > 0)
        {
            targetMapId = mapSections[0].MapId;
        }
        
        if (!string.IsNullOrEmpty(targetMapId) && mapDictionary.ContainsKey(targetMapId))
        {
            // 初始化所有地图为禁用状态（除了起始地图）
            foreach (var map in mapSections)
            {
                if (map.MapId != targetMapId)
                {
                    map.Deactivate();
                }
            }
            
            // 激活起始地图
            MapSection startMap = mapDictionary[targetMapId];
            startMap.Activate();
            currentMap = startMap;
            
            // 定位玩家
            Vector3? spawnPos = startMap.GetSpawnPointPosition(targetSpawnId);
            if (spawnPos.HasValue)
            {
                currentPlayer.transform.position = spawnPos.Value;
                playerMapData.currentMapId = targetMapId;
                playerMapData.currentSpawnPointId = targetSpawnId;
                playerMapData.position = spawnPos.Value;
            }
        }
    }
    
    private MapSpawnPoint FindSpawnPoint(string mapId, string spawnPointId)
    {
        if (!mapDictionary.ContainsKey(mapId)) return null;
        
        MapSection map = mapDictionary[mapId];
        var spawnPoints = map.GetComponentsInChildren<MapSpawnPoint>(true);
        
        foreach (var sp in spawnPoints)
        {
            if (sp.SpawnPointId == spawnPointId)
                return sp;
        }
        
        return null;
    }
    
    /// <summary>
    /// 更新摄像机位置，使其跟随玩家传送
    /// </summary>
    private void UpdateCameraPosition(Vector3 spawnPosition, Vector3 previousPosition)
    {
        // 方法1: 如果设置了Cinemachine虚拟摄像机，使用OnTargetObjectWarped
        if (cinemachineCamera != null)
        {
            // 使用反射调用OnTargetObjectWarped方法（避免直接依赖Cinemachine命名空间）
            var method = cinemachineCamera.GetType().GetMethod("OnTargetObjectWarped", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null && currentPlayer != null)
            {
                method.Invoke(cinemachineCamera, new object[] { currentPlayer.transform, spawnPosition - previousPosition });
                return;
            }
        }
        
        // 方法2: 尝试自动查找Cinemachine虚拟摄像机
        var vcam = FindFirstObjectByType<MonoBehaviour>();
        if (vcam != null && vcam.GetType().Name.Contains("Cinemachine"))
        {
            var method = vcam.GetType().GetMethod("OnTargetObjectWarped", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null && currentPlayer != null)
            {
                method.Invoke(vcam, new object[] { currentPlayer.transform, spawnPosition - previousPosition });
                return;
            }
        }
        
        // 方法3: 直接移动主摄像机（最简单的方法）
        if (mainCameraTransform != null)
        {
            Vector3 offset = spawnPosition - previousPosition;
            mainCameraTransform.position += offset;
        }
        else
        {
            // 如果没有设置，尝试使用Camera.main
            if (Camera.main != null)
            {
                Vector3 offset = spawnPosition - previousPosition;
                Camera.main.transform.position += offset;
            }
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// 获取当前地图
    /// </summary>
    public MapSection GetCurrentMap()
    {
        return currentMap;
    }
    
    /// <summary>
    /// 获取指定ID的地图
    /// </summary>
    public MapSection GetMap(string mapId)
    {
        if (mapDictionary.TryGetValue(mapId, out MapSection map))
            return map;
        return null;
    }
    
    /// <summary>
    /// 设置当前地图（不移动玩家）
    /// </summary>
    public void SetCurrentMap(string mapId)
    {
        if (mapDictionary.TryGetValue(mapId, out MapSection map))
        {
            currentMap = map;
            playerMapData.currentMapId = mapId;
        }
    }
    
    /// <summary>
    /// 检查是否正在传送中
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    
    /// <summary>
    /// 获取玩家数据
    /// </summary>
    public PlayerMapData GetPlayerMapData()
    {
        return playerMapData;
    }
    
    /// <summary>
    /// 初始化地图（查找场景中所有地图）
    /// </summary>
    private void InitializeMaps()
    {
        // 如果列表为空，自动查找场景中所有MapSection
        if (mapSections.Count == 0)
        {
            mapSections.AddRange(FindObjectsByType<MapSection>(FindObjectsSortMode.None));
        }
        
        // 注册所有地图
        foreach (var map in mapSections)
        {
            RegisterMapSection(map);
        }
    }
    
    #endregion
    
    #region Events
    
    public event System.Action<string, string> OnMapTransitionStart; // fromMapId, toMapId
    public event System.Action<string> OnMapTransitionComplete; // toMapId
    
    #endregion
    
    #region Debug
    
    private void OnGUI()
    {
        #if UNITY_EDITOR
        if (GUILayout.Button("测试传送到随机地图"))
        {
            if (mapSections.Count > 1 && currentMap != null)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, mapSections.Count);
                } while (mapSections[randomIndex] == currentMap);
                
                TransitionToMap(mapSections[randomIndex].MapId);
            }
        }
        #endif
    }
    
    #endregion
}