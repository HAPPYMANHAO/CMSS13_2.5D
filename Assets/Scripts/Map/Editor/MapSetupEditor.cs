#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// 地图设置编辑器工具
/// </summary>
public class MapSetupEditor : EditorWindow
{
    private GameObject selectedGrid;
    private string mapId = "Map_01";
    private string mapName = "地图1";
    private bool isStartingMap = false;
    
    [MenuItem("Tools/地图/设置选中的Grid为地图 %#m")]
    public static void SetupSelectedGrid()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("错误", "请先在Hierarchy中选择一个Grid对象！", "确定");
            return;
        }
        
        // 检查是否已有MapSection
        MapSection existingSection = selected.GetComponent<MapSection>();
        if (existingSection != null)
        {
            EditorUtility.DisplayDialog("提示", $"'{selected.name}' 已经是地图区域！\nMapID: {existingSection.MapId}", "确定");
            return;
        }
        
        // 显示设置窗口
        MapSetupEditor window = GetWindow<MapSetupEditor>("地图设置");
        window.selectedGrid = selected;
        window.mapId = selected.name;
        window.mapName = selected.name;
        window.Show();
    }
    
    [MenuItem("Tools/地图/为选中物体添加传送点 %#t")]
    public static void AddTransitionToSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择一个GameObject！", "确定");
            return;
        }
        
        // 创建传送点GameObject
        GameObject transitionObj = new GameObject($"Transition_{selected.name}");
        transitionObj.transform.position = selected.transform.position;
        transitionObj.transform.SetParent(selected.transform);
        
        // 添加碰撞体
        BoxCollider boxCollider = transitionObj.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(2f, 2f, 2f);
        
        // 添加传送组件
        MapTransition transition = transitionObj.AddComponent<MapTransition>();
        
        // 选中新建的物体
        Selection.activeGameObject = transitionObj;
        
        EditorUtility.DisplayDialog("完成", "传送点已创建！\n请在Inspector中设置目标地图ID。", "确定");
    }
    
    [MenuItem("Tools/地图/为选中物体添加生成点 %#s")]
    public static void AddSpawnPointToSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择一个GameObject！", "确定");
            return;
        }
        
        // 创建生成点GameObject
        GameObject spawnObj = new GameObject($"SpawnPoint_{selected.name}");
        spawnObj.transform.position = selected.transform.position;
        spawnObj.transform.SetParent(selected.transform);
        
        // 添加生成点组件
        MapSpawnPoint spawnPoint = spawnObj.AddComponent<MapSpawnPoint>();
        
        // 选中新建的物体
        Selection.activeGameObject = spawnObj;
        
        EditorUtility.DisplayDialog("完成", "生成点已创建！", "确定");
    }
    
    [MenuItem("Tools/地图/自动设置所有Tilemap碰撞 %#c")]
    public static void SetupAllTilemapCollisions()
    {
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        int count = 0;
        
        foreach (var tilemap in tilemaps)
        {
            GameObject go = tilemap.gameObject;
            
            // 检查是否包含"Wall"或"Collision"等关键词
            if (go.name.Contains("Wall") || go.name.Contains("Collision") || 
                go.name.Contains("Collider") || go.name.Contains("障碍"))
            {
                SetupTilemapCollision(go);
                count++;
            }
        }
        
        EditorUtility.DisplayDialog("完成", $"已设置 {count} 个Tilemap的碰撞！", "确定");
    }
    
    private static void SetupTilemapCollision(GameObject tilemapObject)
    {
        // 添加Rigidbody2D (Static)
        Rigidbody2D rb = tilemapObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = tilemapObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.gravityScale = 0;
        }
        
        // 添加TilemapCollider2D
        TilemapCollider2D tileCollider = tilemapObject.GetComponent<TilemapCollider2D>();
        if (tileCollider == null)
        {
            tileCollider = tilemapObject.AddComponent<TilemapCollider2D>();
        }
        tileCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        
        // 添加CompositeCollider2D
        CompositeCollider2D compositeCollider = tilemapObject.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            compositeCollider = tilemapObject.AddComponent<CompositeCollider2D>();
            compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }
    }
    
    private void OnGUI()
    {
        if (selectedGrid == null)
        {
            EditorGUILayout.LabelField("请先选择一个Grid对象");
            return;
        }
        
        EditorGUILayout.LabelField($"选中的对象: {selectedGrid.name}", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        mapId = EditorGUILayout.TextField("地图ID:", mapId);
        mapName = EditorGUILayout.TextField("地图名称:", mapName);
        isStartingMap = EditorGUILayout.Toggle("起始地图:", isStartingMap);
        
        EditorGUILayout.Space(20);
        
        if (GUILayout.Button("设置地图", GUILayout.Height(40)))
        {
            SetupMap();
        }
    }
    
    private void SetupMap()
    {
        // 添加MapSection组件
        MapSection section = selectedGrid.AddComponent<MapSection>();
        
        // 使用反射设置私有字段（简化版本，实际应该用 SerializedObject）
        SerializedObject serializedSection = new SerializedObject(section);
        serializedSection.FindProperty("mapId").stringValue = mapId;
        serializedSection.FindProperty("mapName").stringValue = mapName;
        serializedSection.FindProperty("isStartingMap").boolValue = isStartingMap;
        serializedSection.ApplyModifiedProperties();
        
        // 自动查找Tilemap
        Tilemap[] tilemaps = selectedGrid.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            // 根据名字自动分配
            if (tilemap.name.Contains("Ground") || tilemap.name.Contains("地面"))
            {
                serializedSection.FindProperty("groundTilemap").objectReferenceValue = tilemap;
            }
            else if (tilemap.name.Contains("Wall") || tilemap.name.Contains("墙") || tilemap.name.Contains("障碍"))
            {
                serializedSection.FindProperty("wallTilemap").objectReferenceValue = tilemap;
            }
            else if (tilemap.name.Contains("Decoration") || tilemap.name.Contains("装饰"))
            {
                serializedSection.FindProperty("decorationTilemap").objectReferenceValue = tilemap;
            }
        }
        serializedSection.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(selectedGrid);
        
        Close();
        
        EditorUtility.DisplayDialog("完成", $"'{selectedGrid.name}' 已设置为地图区域！\nMapID: {mapId}", "确定");
    }
}
#endif