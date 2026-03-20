# 后坐力相机摇晃效果设置指南

## 工作原理

**只在射击时摇晃！** 不是持续摇晃。

1. **射击触发**: 每次开火时触发一次摇晃
2. **强度基于后坐力**: 当前积累的后坐力越高，摇晃越剧烈
3. **自动衰减**: 摇晃会在指定时间内逐渐减弱直至停止
4. **回到原位**: 摇晃结束后相机平滑回到原始位置

## 快速设置步骤

### 1. 添加RecoilCameraShake脚本
1. 在战斗场景中创建一个空物体，命名为 "CameraShakeManager"
2. 添加 **RecoilCameraShake** 脚本
3. 在 Inspector 中配置：
   - **Battle Camera**: 拖入你的主相机（通常自动识别Main Camera）
   - **Battle Entity Manager**: 拖入 BattleEntityManager

### 2. 调整摇晃参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| Min Shake Amplitude | 0.1 | 后坐力0时的摇晃强度 |
| Max Shake Amplitude | 1.0 | 后坐力100时的最大摇晃强度 |
| Min Shake Duration | 0.1s | 后坐力0时的摇晃持续时间 |
| Max Shake Duration | 0.5s | 后坐力100时的最大摇晃持续时间 |
| Shake Frequency | 20 | 摇晃频率，越高越快 |

## 摇晃行为

### 后坐力影响
- **后坐力0**: 轻微摇晃，持续0.1秒
- **后坐力50**: 中等摇晃，持续0.3秒
- **后坐力100**: 剧烈摇晃，持续0.5秒

### 衰减曲线
使用 **Decay Curve** 可以自定义摇晃如何随时间减弱：
- **默认(EaseInOut)**: 开始强，然后逐渐减弱
- **Linear**: 线性均匀减弱
- **EaseIn**: 开始弱，后来强，然后突然停止
- **自定义**: 根据你的游戏感觉调整

## 技术细节

### 摇晃模拟
- 使用柏林噪声生成自然的随机摇晃
- 添加向上的分量模拟枪管上扬
- 摇晃结束后平滑回到原位

### 与游戏系统集成
- 自动监听 `GunFireAction.OnGunFired` 事件
- 从 `PartyBattleEntity.accumulatedRecoil` 读取当前后坐力
- 战斗结束自动重置

## 调试

在Unity编辑器中运行游戏时，屏幕左上角显示：
- 当前后坐力值 (0-100)
- 是否正在摇晃
- 当前摇晃强度
- 剩余摇晃时间

## 手动触发

如果你想手动触发摇晃（例如爆炸、受击等）：

```csharp
RecoilCameraShake shake = FindFirstObjectByType<RecoilCameraShake>();
shake.TriggerShake();
```

## 注意事项

1. ✅ 每次射击触发一次摇晃
2. ✅ 摇晃强度和持续时间基于当前后坐力
3. ✅ 摇晃自动衰减并回到原位
4. ✅ 不影响相机的原始位置和旋转
