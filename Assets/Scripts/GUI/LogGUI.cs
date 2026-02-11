using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogGUI : MonoBehaviour
{
    [SerializeField] private GameObject logItemPrefab; 
    [SerializeField] private Transform contentTransform; 
    [SerializeField] private ScrollRect scrollRect; 

    [SerializeField] private Color logColorDefault = Color.white;

    private const int MAX_LOG_COUNT = LogTextSettings.MAX_LOG_COUNT; 
    private Queue<GameObject> logQueue = new Queue<GameObject>();

    private void Start()
    {
        ActionBase.OnActionLogged += HandleActionLogged;
        BattleTurnManager.OnBattleTurnLoggedColor += HandleTurnLogged;
    }
    private void OnDisable()
    {
        ActionBase.OnActionLogged -= HandleActionLogged;
        BattleTurnManager.OnBattleTurnLoggedColor -= HandleTurnLogged;
    }

    public void UpdateLog(string message)
    {
        LogMessage( message, logColorDefault);
    }
    public void UpdateLog(string message, Color color)
    {
        LogMessage(message, color);
    }

    private void LogMessage(string message, Color color)
    {
        if (this == null || scrollRect == null) return;

        GameObject newLog = Instantiate(logItemPrefab, contentTransform);
        var textComp = newLog.GetComponent<TextMeshProUGUI>();
        textComp.text = message;
        textComp.color = color;

        logQueue.Enqueue(newLog);

        if (logQueue.Count > MAX_LOG_COUNT)
        {
            Destroy(logQueue.Dequeue());
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            contentTransform as RectTransform
        );
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    //------------------Event-------------------//
    private void HandleActionLogged(string log)
    {
        UpdateLog(log);
    }
    private void HandleTurnLogged(string log, Color color)
    {
        UpdateLog(log, color);
    }
}
