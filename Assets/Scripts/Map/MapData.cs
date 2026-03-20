using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图区域数据 - 存储单个地图的状态
/// </summary>
[Serializable]
public class MapSectionData
{
    public string mapId;
    public string mapName;
    public bool isUnlocked = true;
    public bool isVisited = false;
    
    // 可以存储地图特定的数据（如已开启的门、已击败的敌人等）
    public List<string> completedEvents = new List<string>();
    
    public MapSectionData(string id, string name)
    {
        mapId = id;
        mapName = name;
    }
}

/// <summary>
/// 传送点数据
/// </summary>
[Serializable]
public class MapTransitionData
{
    public string transitionId;
    public string fromMapId;
    public string toMapId;
    public string spawnPointId; // 目标地图的生成点ID
}

/// <summary>
/// 玩家在地图中的位置数据
/// </summary>
[Serializable]
public class PlayerMapData
{
    public string currentMapId;
    public string currentSpawnPointId;
    public Vector3 position;
    public Vector3 facingDirection = Vector3.back;
}