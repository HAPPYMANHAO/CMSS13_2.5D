using Unity.Mathematics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerEventTrigger : MonoBehaviour
{
    [SerializeField] PartyManager partyManager;
    GameSceneManager gameSceneManager;  

    [SerializeField] private LayerMask battleTriggerLayer;

    private bool moveInDangerArea;
    private float enemyEncounterTimer = 0f;
    [SerializeField] private int stepInDangerArea;
    [SerializeField] private int maxStepsToEncounter = 4;
    [SerializeField] private int minStepsToEncounter = 2;

    private int stepsToEncounter;

    private const float TIME_PER_STEP = 0.5f;

    private void Awake()
    {
        CalculateStepsToNextEncounter();

        gameSceneManager = GameObject.FindFirstObjectByType<GameSceneManager>();    
    }

    private void Start()
    {
        if(partyManager.GetPlayerPosition() != Vector3.zero)
        {
            this.transform.position = partyManager.GetPlayerPosition();
        }
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f, battleTriggerLayer);

        if (colliders.Length > 0) moveInDangerArea = true;
        else moveInDangerArea = false;

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
                    gameSceneManager.EnterBattle();
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
