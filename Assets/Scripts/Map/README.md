# 地图管理系统使用说明

## 📁 文件结构

```
Scripts/Map/
├── MapManager.cs           # 核心管理器（单例）
├── MapSection.cs           # 地图区域组件
├── MapSpawnPoint.cs        # 玩家生成点
├── MapTransition.cs        # 地图传送点
├── MapTrigger.cs           # 通用触发器
├── MapData.cs              # 数据类
└── Editor/
    └── MapSetupEditor.cs   # 编辑器工具
```

## 🚀 快速开始

### 1. 场景设置

#### 创建地图区域

1. 在场景中创建 **Grid** 对象（包含Tilemap）
2. 选中Grid，使用菜单 `Tools > 地图 > 设置选中的Grid为地图` (Ctrl+Shift+M)
3. 设置地图ID和名称

#### 添加传送点

1. 选择一个GameObject作为传送点位置
2. 使用菜单 `Tools > 地图 > 为选中物体添加传送点` (Ctrl+Shift+T)
3. 在Inspector中设置：
   - **Target Map Id**: 目标地图的ID
   - **Target Spawn Point Id**: 目标地图的生成点ID（可选）

#### 添加生成点

1. 在地图内选择一个位置
2. 使用菜单 `Tools > 地图 > 为选中物体添加生成点` (Ctrl+Shift+S)
3. 设置生成点ID和朝向

### 2. 组件说明

#### MapManager（必需）

挂在任意GameObject上（建议空物体）

**配置：**
- **Player Prefab**: 玩家预制体（可选）
- **Starting Map Id**: 起始地图ID
- **Starting Spawn Point Id**: 起始生成点ID
- **Loading Screen Canvas**: 加载界面（可选）

#### MapSection（每个地图一个）

自动添加，包含：
- **Map Id**: 唯一标识符
- **Map Name**: 显示名称
- **Ground/ Wall/ Decoration Tilemap**: 地图层级引用

#### MapTransition（传送点）

**配置：**
- **Target Map Id**: 目标地图ID（必需）
- **Target Spawn Point Id**: 目标生成点ID
- **Require Player Tag**: 是否只响应Player标签
- **Only Trigger Once**: 是否只触发一次
- **Use Fade Effect**: 是否使用淡入淡出效果

### 3. 工作流程

```
玩家进入传送点触发器
        ↓
MapTransition.TriggerTransition()
        ↓
MapManager.TransitionToMap()
        ↓
显示加载界面（可选）
        ↓
禁用当前地图
激活目标地图
移动玩家到生成点
        ↓
隐藏加载界面
触发传送完成事件
```

## 🎨 加载界面设置

1. 创建Canvas，添加半透明黑色背景
2. 添加加载文字（可选）
3. 添加MapManager引用
4. 配置淡入淡出时间

## 🔧 高级用法

### 手动触发传送

```csharp
// 通过代码传送到指定地图
MapManager.Instance.TransitionToMap(
    "Map_02",           // 目标地图ID
    "Spawn_Entry",      // 生成点ID（可选）
    true,               // 使用淡入淡出
    0.5f                // 淡入淡出时间
);
```

### 监听传送事件

```csharp
void OnEnable()
{
    MapManager.Instance.OnMapTransitionStart += OnTransitionStart;
    MapManager.Instance.OnMapTransitionComplete += OnTransitionComplete;
}

void OnTransitionStart(string fromMapId, string toMapId)
{
    Debug.Log($"从 {fromMapId} 传送到 {toMapId}");
}

void OnTransitionComplete(string mapId)
{
    Debug.Log($"已到达 {mapId}");
}
```

### 地图触发器

用于触发各种地图事件（非传送）：

```csharp
// 进入区域时触发战斗
MapTrigger trigger = GetComponent<MapTrigger>();
trigger.OnTrigger.AddListener((player) => {
    EncounterManager.Instance.StartBattle();
});
```

## ⚠️ 注意事项

1. **标签设置**: 确保玩家GameObject有"Player"标签
2. **碰撞体**: 传送点必须有Collider并勾选IsTrigger
3. **唯一ID**: 每个地图的MapId必须唯一
4. **场景持久化**: MapManager使用DontDestroyOnLoad

## 🔍 调试

在编辑器中运行时可以：
- 查看地图边界Gizmos（青色线框）
- 查看生成点Gizmos（绿色球体）
- 查看传送点Gizmos（青色半透明框）
- 使用GUI按钮测试随机传送

## 📌 最佳实践

1. **命名规范**:
   - 地图ID: `Map_01`, `Forest_Area`, `Dungeon_Level1`
   - 生成点ID: `Spawn_Entry`, `Spawn_BossRoom`, `Spawn_Shop`
   
2. **场景组织**:
   ```
   Scene/
   ├── Grid (MapSection)
   │   ├── Ground (Tilemap)
   │   ├── Walls (Tilemap + Collider)
   │   ├── Decorations (Tilemap)
   │   ├── SpawnPoints/
   │   │   ├── Spawn_Entry
   │   │   └── Spawn_Exit
   │   └── Transitions/
   │       ├── To_Forest
   │       └── To_Dungeon
   ```

3. **性能优化**:
   - 不活动的地图会自动禁用
   - 使用Composite Collider 2D合并碰撞体
   - 只在必要时启用地图的GameObject

## 🐛 常见问题

**Q: 传送后玩家位置不对？**  
A: 检查目标地图的生成点ID是否正确，或检查生成点是否在该地图内。

**Q: 传送没有效果？**  
A: 确保传送点的Collider是Trigger，玩家有"Player"标签。

**Q: 地图不显示？**  
A: 检查MapSection是否正确挂在Grid上，MapId是否设置。

**Q: 碰撞体太卡？**  
A: 使用 `Tools > 地图 > 自动设置所有Tilemap碰撞` 来添加Composite Collider。
