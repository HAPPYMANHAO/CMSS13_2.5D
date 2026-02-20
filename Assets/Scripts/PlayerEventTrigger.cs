using Unity.Mathematics;
using Unity.Properties;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlayerEventTrigger : MonoBehaviour
{
    [SerializeField] PartyManager partyManager;

    [SerializeField] private LayerMask battleTriggerLayer;

    private bool moveInDangerArea;
    private float enemyEncounterTimer = 0f;
    [SerializeField] private int stepInDangerArea;
    [SerializeField] private int maxStepsToEncounter = 4;
    [SerializeField] private int minStepsToEncounter = 2;

    private int stepsToEncounter;

    private const float TIME_PER_STEP = 0.5f;
    private const string BATTLE_SCENE = "BattleScene";

    private void Awake()
    {
        CalculateStepsToNextEncounter();
    }

    private void Start()
    {
        if(partyManager.GetPlayerPositon() != Vector3.zero)
        {
            this.transform.position = partyManager.GetPlayerPositon();
        }
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f, battleTriggerLayer);

        if (colliders.Length > 0) moveInDangerArea = true;

        if (moveInDangerArea)
        {
            enemyEncounterTimer += Time.deltaTime;
            if (enemyEncounterTimer > TIME_PER_STEP)
            {
                stepInDangerArea++;
                enemyEncounterTimer -= TIME_PER_STEP;

                if (stepInDangerArea >= stepsToEncounter)
                {
                    partyManager.SetPlayerPosition(this.transform.position);
                    stepInDangerArea = 0;
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }
        else
        {
            enemyEncounterTimer = 0f;
        }
    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = UnityEngine.Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}
