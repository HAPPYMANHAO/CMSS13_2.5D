using UnityEngine;

/// <summary>
/// 后坐力相机摇晃效果 - 每次射击时触发摇晃，然后随时间衰减
/// 摇晃强度和持续时间基于当前后坐力值
/// </summary>
public class RecoilCameraShake : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Camera battleCamera;
    [SerializeField] private BattleEntityManager battleEntityManager;

    [Header("Shake Settings")]
    [Tooltip("后坐力为0时的摇晃强度")]
    [SerializeField] private float minShakeAmplitude = 0.1f;
    
    [Tooltip("后坐力为100时的最大摇晃强度")]
    [SerializeField] private float maxShakeAmplitude = 1.0f;
    
    [Tooltip("后坐力为0时的摇晃持续时间")]
    [SerializeField] private float minShakeDuration = 0.1f;
    
    [Tooltip("后坐力为100时的最大摇晃持续时间")]
    [SerializeField] private float maxShakeDuration = 0.5f;
    
    [Tooltip("摇晃频率")]
    [SerializeField] private float shakeFrequency = 20f;

    [Header("Recoil Influence")]
    [Tooltip("后坐力对摇晃的影响曲线")]
    [SerializeField] private AnimationCurve recoilInfluenceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Decay Curve")]
    [Tooltip("摇晃衰减曲线（X:时间(0-1), Y:强度倍数(1-0)）")]
    [SerializeField] private AnimationCurve decayCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    // 当前后坐力值（0-100）- 用于计算摇晃强度
    private float currentRecoil;
    
    // 摇晃状态
    private bool isShaking = false;
    private float shakeTimer = 0f;
    private float currentShakeDuration = 0f;
    private float currentShakeIntensity = 0f;
    
    // 相机原始位置和旋转
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform cameraTransform;
    
    // 噪声种子（用于生成不同随机模式）
    private Vector3 noiseSeed;

    private void Awake()
    {
        if (battleCamera == null)
        {
            battleCamera = Camera.main;
        }
        
        if (battleEntityManager == null)
        {
            battleEntityManager = FindFirstObjectByType<BattleEntityManager>();
        }

        if (battleCamera != null)
        {
            cameraTransform = battleCamera.transform;
            originalPosition = cameraTransform.localPosition;
            originalRotation = cameraTransform.localRotation;
        }
        
        // 初始化随机种子
        noiseSeed = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    private void OnEnable()
    {
        BattleEntityManager.OnPartyEntitiesSpawned += OnPartyEntitiesSpawned;
        GunFireAction.OnGunFired += OnGunFired;
    }

    private void OnDisable()
    {
        BattleEntityManager.OnPartyEntitiesSpawned -= OnPartyEntitiesSpawned;
        GunFireAction.OnGunFired -= OnGunFired;
    }

    private void OnPartyEntitiesSpawned()
    {
        // 战斗开始，记录初始位置
        if (cameraTransform != null)
        {
            originalPosition = cameraTransform.localPosition;
            originalRotation = cameraTransform.localRotation;
        }
    }

    private void Update()
    {
        UpdateRecoilFromPlayer();
        
        if (isShaking)
        {
            UpdateShake();
        }
        else
        {
            // 确保相机回到原位
            ReturnToOriginalPosition();
        }
    }

    /// <summary>
    /// 从当前玩家实体获取后坐力值
    /// </summary>
    private void UpdateRecoilFromPlayer()
    {
        if (battleEntityManager?.currentPlayerEntity is PartyBattleEntity partyEntity)
        {
            currentRecoil = Mathf.Clamp(partyEntity.accumulatedRecoil, 0, 100);
        }
        else
        {
            currentRecoil = 0;
        }
    }

    /// <summary>
    /// 射击时触发摇晃
    /// </summary>
    private void OnGunFired()
    {
        TriggerShake();
    }

    /// <summary>
    /// 触发相机摇晃
    /// </summary>
    public void TriggerShake()
    {
        // 计算基于当前后坐力的摇晃参数
        float recoilNormalized = currentRecoil / 100f;
        float influence = recoilInfluenceCurve.Evaluate(recoilNormalized);
        
        // 设置摇晃强度和持续时间
        currentShakeIntensity = Mathf.Lerp(minShakeAmplitude, maxShakeAmplitude, influence);
        currentShakeDuration = Mathf.Lerp(minShakeDuration, maxShakeDuration, influence);
        
        // 开始摇晃
        shakeTimer = currentShakeDuration;
        isShaking = true;
        
        // 生成新的随机种子，使每次摇晃不同
        noiseSeed = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );
    }

    /// <summary>
    /// 更新摇晃效果
    /// </summary>
    private void UpdateShake()
    {
        if (cameraTransform == null) return;

        // 减少计时器
        shakeTimer -= Time.deltaTime;
        
        if (shakeTimer <= 0)
        {
            // 摇晃结束
            isShaking = false;
            shakeTimer = 0;
            return;
        }

        // 计算当前进度 (0-1)
        float progress = 1f - (shakeTimer / currentShakeDuration);
        
        // 根据衰减曲线计算当前强度
        float decayMultiplier = decayCurve.Evaluate(progress);
        float currentAmplitude = currentShakeIntensity * decayMultiplier;

        // 使用柏林噪声生成摇晃
        float time = Time.time * shakeFrequency;
        
        float noiseX = Mathf.PerlinNoise(noiseSeed.x + time, noiseSeed.y) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(noiseSeed.y + time, noiseSeed.z) * 2f - 1f;
        float noiseZ = Mathf.PerlinNoise(noiseSeed.z + time, noiseSeed.x) * 2f - 1f;

        // 应用摇晃偏移
        Vector3 shakeOffset = new Vector3(
            noiseX * currentAmplitude,
            noiseY * currentAmplitude,
            noiseZ * currentAmplitude * 0.3f  // Z轴摇晃更小
        );

        // 后坐力主要向上和向后（模拟枪械后坐力）
        shakeOffset.y += currentAmplitude * 0.5f; // 向上的分量
        
        // 应用位置和旋转
        cameraTransform.localPosition = originalPosition + shakeOffset;

        // 添加轻微的旋转摇晃
        Vector3 rotationEuler = new Vector3(
            -currentAmplitude * 5f,  // 向上的旋转（枪管上扬）
            noiseX * currentAmplitude * 3f,
            0
        );
        cameraTransform.localRotation = originalRotation * Quaternion.Euler(rotationEuler);
    }

    /// <summary>
    /// 将相机平滑回到原始位置
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        if (cameraTransform == null) return;

        // 使用Lerp平滑回到原位
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition, 
            originalPosition, 
            Time.deltaTime * 10f
        );
        
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation, 
            originalRotation, 
            Time.deltaTime * 10f
        );
    }

    /// <summary>
    /// 重置摇晃效果
    /// </summary>
    public void ResetShake()
    {
        isShaking = false;
        shakeTimer = 0;
        currentShakeIntensity = 0;
        currentShakeDuration = 0;
        currentRecoil = 0;
        
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = originalPosition;
            cameraTransform.localRotation = originalRotation;
        }
    }

    /// <summary>
    /// 获取当前后坐力百分比
    /// </summary>
    public float GetCurrentRecoilPercent()
    {
        return currentRecoil;
    }

    /// <summary>
    /// 检查是否正在摇晃
    /// </summary>
    public bool IsShaking()
    {
        return isShaking;
    }

    private void OnDestroy()
    {
        ResetShake();
    }

    // 用于调试的可视化
    private void OnGUI()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && battleEntityManager?.currentPlayerEntity != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 250, 120));
            GUILayout.Label($"Recoil: {currentRecoil:F1}/100");
            GUILayout.Label($"Shaking: {(isShaking ? "Yes" : "No")}");
            if (isShaking)
            {
                GUILayout.Label($"Intensity: {currentShakeIntensity:F3}");
                GUILayout.Label($"Time Left: {shakeTimer:F2}s");
            }
            GUILayout.EndArea();
        }
#endif
    }
}
