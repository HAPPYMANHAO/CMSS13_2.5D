using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;

    private static GameObject instance;//self

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

    public void ChangeToBattleScene()
    {
        gameStateManager.currentGameState = GameState.Battle;
        SceneManager.LoadScene(SceneName.BATTLE);
    }
    public void ChangeToOverworldScene()
    {
        gameStateManager.currentGameState = GameState.Overworld;
        SceneManager.LoadScene(SceneName.OVER_WORLD);
    }
}
