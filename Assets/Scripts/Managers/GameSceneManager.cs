using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;

    private static GameObject instance;//self

    [SerializeField] private Camera overworldCamera;
    private OverworldVisualGUI overworldUI;
    [SerializeField] GameObject Player;
    private PlayerController playerController;
    private PlayerEventTrigger playerEventTrigger;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this.gameObject;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        overworldUI = FindFirstObjectByType<OverworldVisualGUI>();
        playerController = Player.GetComponent<PlayerController>();
        playerEventTrigger = Player.GetComponent<PlayerEventTrigger>();
    }

    public IEnumerator EnterBattle()
    {
        gameStateManager.currentGameState = GameState.Battle;
        overworldCamera.gameObject.SetActive(false);
        overworldUI.gameObject.SetActive(false);
        playerController.enabled = false;
        playerEventTrigger.enabled = false;

        yield return SceneManager.LoadSceneAsync(SceneName.BATTLE, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName.BATTLE));
    }
    public IEnumerator ChangeToOverworldScene()
    {
        Scene overworld = SceneManager.GetSceneByName(SceneName.OVER_WORLD);

        if (overworld.IsValid())
        {
            SceneManager.SetActiveScene(overworld);
        }

        gameStateManager.currentGameState = GameState.Overworld;
        yield return SceneManager.UnloadSceneAsync(SceneName.BATTLE);

        overworldCamera.gameObject.SetActive(true);
        overworldUI.gameObject.SetActive(true);
        playerController.enabled = true;
        playerEventTrigger.enabled = true;

        Debug.Log("Overworld Camera Active: " + overworldCamera.gameObject.activeSelf);
        Debug.Log("Active Scene: " + SceneManager.GetActiveScene().name);
    }

    public void ExitBattle()
    {
        // 由 GameSceneManager 启动协程
        StartCoroutine(ChangeToOverworldScene());
    }
}
