using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncountEnemy[] encountEnemies;
         
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        
    }

    private void Start()
    {
        enemyManager.SpawnEnemeisByCounter(encountEnemies);
    }
}


[System.Serializable]
public class EncountEnemy
{
    public EnemyInfo enemy;
}
