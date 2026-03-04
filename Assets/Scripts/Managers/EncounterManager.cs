using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncountEnemy[] encounterEnemies;
         
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = EnemyManager.instance;
        
    }

    private void Start()
    {
        enemyManager.SpawnEnemiesByEncounter(encounterEnemies);
    }
}


[System.Serializable]
public class EncountEnemy
{
    public EnemyInfo enemy;
}
