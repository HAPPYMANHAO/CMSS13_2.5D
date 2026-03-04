using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentGameState;

    public Action<GameState> gameStateChanged;

    public static GameStateManager instance;//self

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
}
