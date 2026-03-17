using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActionQueueGUI : MonoBehaviour
{
    [SerializeField] EnemyActionGUI enemyActionPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;

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
                Destroy(icon.gameObject); 
            }
        }
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

