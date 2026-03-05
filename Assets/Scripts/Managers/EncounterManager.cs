using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncountEnemy[] encounterEnemies;
         
    private EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = EnemyManager.instance;
        enemyManager.SpawnEnemiesByEncounter(encounterEnemies);
    }
}


[System.Serializable]
public class EncountEnemy
{
    public EnemyInfo enemy;
}
