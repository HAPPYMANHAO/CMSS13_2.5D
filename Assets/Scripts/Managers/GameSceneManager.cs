using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;

    public static GameSceneManager instance;//self

    [SerializeField] private Camera overworldCamera;
    private OverworldVisualGUI overworldUI;
    [SerializeField] GameObject Player;
    private PlayerController playerController;
    private PlayerEventTrigger playerEventTrigger;

    [Header("Loading Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // 淡入淡出持续时间
    [SerializeField] private float minimumLoadingTime = 1.5f; // 最少显示加载界面1.5秒，避免闪烁
    [SerializeField] private float progressSmoothSpeed = 5f; // 进度条平滑速度

    private bool isLoading = false;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        overworldUI = FindFirstObjectByType<OverworldVisualGUI>();
        playerController = Player.GetComponent<PlayerController>();
        playerEventTrigger = Player.GetComponent<PlayerEventTrigger>();
    }

    public void EnterBattle()
    {
        if (isLoading) return;
        StartCoroutine(EnterBattleCoroutine());
    }

    private IEnumerator EnterBattleCoroutine()
    {
        isLoading = true;
        float loadingStartTime = Time.time;
        float fadeTime = fadeDuration;

        // 显示加载界面并等待淡入完成
        if (LoadingScreenGUI.Instance != null)
        {
            bool showComplete = false;
            LoadingScreenGUI.Instance.Show(fadeTime, () => showComplete = true);
            yield return new WaitUntil(() => showComplete);
        }

        // 禁用 Overworld 相关组件
        gameStateManager.currentGameState = GameState.Battle;
        if (overworldCamera != null)
            overworldCamera.gameObject.SetActive(false);
        if (overworldUI != null)
            overworldUI.gameObject.SetActive(false);
        if (playerController != null)
            playerController.enabled = false;
        if (playerEventTrigger != null)
            playerEventTrigger.enabled = false;

        // 异步加载战斗场景
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(SceneName.BATTLE, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false; // 先不激活场景

        float currentProgress = 0f;

        // 等待加载完成
        while (!loadOperation.isDone)
        {
            // 实际加载进度（0-0.9）
            float targetProgress = loadOperation.progress;

            // 如果进度达到 0.9，说明加载完成，等待激活
            if (loadOperation.progress >= 0.9f)
            {
                targetProgress = 1f;
            }

            // 平滑更新进度
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * progressSmoothSpeed);

            // 更新加载界面
            if (LoadingScreenGUI.Instance != null)
            {
                LoadingScreenGUI.Instance.UpdateProgress(currentProgress);
            }

            // 当进度达到 100% 且满足最小加载时间时，激活场景
            if (currentProgress >= 0.99f && loadOperation.progress >= 0.9f)
            {
                float elapsedTime = Time.time - loadingStartTime;
                if (elapsedTime >= minimumLoadingTime)
                {
                    loadOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // 设置活动场景
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName.BATTLE));

        // 给 Battle 场景一帧时间来初始化
        yield return null;

        // 隐藏加载界面
        if (LoadingScreenGUI.Instance != null)
        {
            bool fadeComplete = false;
            LoadingScreenGUI.Instance.Hide(-1f, () => fadeComplete = true);
            yield return new WaitUntil(() => fadeComplete);
        }

        isLoading = false;
    }

    public void ExitBattle()
    {
        if (isLoading) return;
        StartCoroutine(ExitBattleCoroutine());
    }

    private IEnumerator ExitBattleCoroutine()
    {
        isLoading = true;
        float loadingStartTime = Time.time;
        float fadeTime = fadeDuration;

        // 显示加载界面并等待淡入完成
        if (LoadingScreenGUI.Instance != null)
        {
            bool showComplete = false;
            LoadingScreenGUI.Instance.Show(fadeTime, () => showComplete = true);
            yield return new WaitUntil(() => showComplete);
        }

        // 切换游戏状态
        gameStateManager.currentGameState = GameState.Overworld;

        // 获取 Overworld 场景
        Scene overworld = SceneManager.GetSceneByName(SceneName.OVER_WORLD);
        if (overworld.IsValid())
        {
            SceneManager.SetActiveScene(overworld);
        }

        // 异步卸载战斗场景
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneName.BATTLE);

        float currentProgress = 0f;

        // 等待卸载完成
        while (unloadOperation != null && !unloadOperation.isDone)
        {
            // 平滑更新进度
            currentProgress = Mathf.MoveTowards(currentProgress, 1f, Time.deltaTime * progressSmoothSpeed);

            if (LoadingScreenGUI.Instance != null)
            {
                LoadingScreenGUI.Instance.UpdateProgress(currentProgress);
            }

            yield return null;
        }

        // 确保满足最小加载时间
        float elapsedTime = Time.time - loadingStartTime;
        while (elapsedTime < minimumLoadingTime)
        {
            elapsedTime = Time.time - loadingStartTime;
            yield return null;
        }

        // 重新启用 Overworld 组件
        if (overworldCamera != null)
            overworldCamera.gameObject.SetActive(true);
        if (overworldUI != null)
            overworldUI.gameObject.SetActive(true);
        if (playerController != null)
            playerController.enabled = true;
        if (playerEventTrigger != null)
            playerEventTrigger.enabled = true;

        // 隐藏加载界面
        if (LoadingScreenGUI.Instance != null)
        {
            bool fadeComplete = false;
            LoadingScreenGUI.Instance.Hide(-1f, () => fadeComplete = true);
            yield return new WaitUntil(() => fadeComplete);
        }

        isLoading = false;
    }
}
