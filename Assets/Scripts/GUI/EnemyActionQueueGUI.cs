using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActionQueueGUI : MonoBehaviour
{
    [SerializeField] EnemyActionGUI enemyActionPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Animation Settings")]
    [SerializeField] private float exitMoveDuration = 0.3f;
    [SerializeField] private float exitMoveDistance = 100f;
    [SerializeField] private Ease exitMoveEase = Ease.InBack;

    private Dictionary<EnemyBattleEntity, Queue<EnemyActionGUI>> enemyIconQueues =
        new Dictionary<EnemyBattleEntity, Queue<EnemyActionGUI>>();
    private void OnEnable()
    {
        BattleTurnManager.OnEnemyQueueBuilt += HandleOnEnemyQueueBuilt;
        BattleTurnManager.OnEnemyActionExecuted += HandleOnEnemyActionExecuted;
    }
    private void OnDisable()
    {
        BattleTurnManager.OnEnemyQueueBuilt -= HandleOnEnemyQueueBuilt;
        BattleTurnManager.OnEnemyActionExecuted -= HandleOnEnemyActionExecuted;
    }

    private void HandleOnEnemyActionExecuted(EnemyBattleEntity enemy)
    {
        if (enemyIconQueues.TryGetValue(enemy, out Queue<EnemyActionGUI> queue))
        {
            if (queue.Count > 0)
            {
                EnemyActionGUI icon = queue.Dequeue();
                PlayExitAnimationAndDestroy(icon);
            }
        }
    }

    /// <summary>
    /// 播放向左移动的退出动画，然后销毁
    /// </summary>
    private void PlayExitAnimationAndDestroy(EnemyActionGUI icon)
    {
        if (icon == null) return;

        // 获取 RectTransform
        RectTransform rectTransform = icon.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Destroy(icon.gameObject);
            return;
        }

        // 计算目标位置（向左移动）
        Vector2 targetPos = rectTransform.anchoredPosition + new Vector2(-exitMoveDistance, 0);

        // 播放移动动画并销毁
        rectTransform
            .DOAnchorPos(targetPos, exitMoveDuration)
            .SetEase(exitMoveEase)
            .OnComplete(() =>
            {
                if (icon != null && icon.gameObject != null)
                    Destroy(icon.gameObject);
            });
    }

    private void HandleOnEnemyQueueBuilt(System.Collections.Generic.List<(EnemyBattleEntity enemy, ActionBase action)> enemies)
    {
        if (this == null || scrollRect == null) return;

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        enemyIconQueues.Clear();

        foreach (var (enemy, action) in enemies)
        {
            EnemyActionGUI newIcon = Instantiate(enemyActionPrefab, contentTransform);
            newIcon.Setup(enemy, action);

            if (!enemyIconQueues.ContainsKey(enemy))
                enemyIconQueues[enemy] = new Queue<EnemyActionGUI>();
            enemyIconQueues[enemy].Enqueue(newIcon);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform as RectTransform);
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f; // 滚动到底部（可根据需要调整）
        }
    }
}

