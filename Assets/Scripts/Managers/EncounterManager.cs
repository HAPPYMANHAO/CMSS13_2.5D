using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncountEnemy[] encountEnemies;
         
    private EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        enemyManager.SpawnEnemeisByCounter(encountEnemies);
    }
}


[System.Serializable]
public class EncountEnemy
{
    public EnemyInfo enemy;
}
