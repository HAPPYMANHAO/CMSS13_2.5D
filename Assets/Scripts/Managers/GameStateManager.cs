using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentGameState;

    public Action<GameState> gameStateChanged;

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
}
