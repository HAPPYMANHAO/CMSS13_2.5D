using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;

public class BattleTurnManager : MonoBehaviour
{
    [SerializeField] BattleEntityManager battleEntityManager;
    [SerializeField] BattleVisualGUI battleVisualGUI;

    [SerializeField] public BattleState battleState;

    private const int FIRST_PLAYER_ENTITY = 0;

    public enum BattleState
    {
        Setup,
        PlayerTurnStart,
        PlayerTurnAction,
        PlayerTurnAIAutoAction,
        PlayerTurnEnd,
        EnemyTurnStart,
        EnemyTurnAction,
        EnemyTurnEnd,
        Victory,
        Defeat,
        Escape,
        BattleEnd
    }

    private void Start()
    {
        SetUpBattleStart();
    }

    private void Update()
    {
        switch(battleState)
        {
            case BattleState.Victory:
                Debug.Log("win");
                battleState = BattleState.BattleEnd;
                break;
            case BattleState.Defeat:
                battleState = BattleState.BattleEnd;
                break;
            default:
                break;
        }
    }

    private void SetUpBattleStart()
    {
        battleState = BattleState.Setup;

        for (int i = 0; i < battleEntityManager.allBattleEntities.Count; i++)
        {
            battleEntityManager.allBattleEntities[i].entityBattleState = BattleEntityActionStates.Attack;
        }

        battleEntityManager.currentPlayerEntity = FIRST_PLAYER_ENTITY;

        battleState = BattleState.PlayerTurnStart;
    }

    private IEnumerator BattleRoutine()
    {     

        for (int i = 0; i < battleEntityManager.allBattleEntities.Count; i++)
        {
            switch (battleEntityManager.allBattleEntities[i].entityBattleState)
            {
                case BattleEntityActionStates.Attack:
                    break;
                case BattleEntityActionStates.Defend:
                    break;
                case BattleEntityActionStates.Run:
                    break;
                case BattleEntityActionStates.Idle:
                    break;
                case BattleEntityActionStates.Unconscious:
                    break;
                default:
                    break;
            }
        }

        yield return null;
    } 

    private IEnumerator AutoExecuteActionRoutine(int entityIndex)
    {
        BattleEntityBase currentEntity = battleEntityManager.allBattleEntities[entityIndex];
        switch (currentEntity.entityFaction)
        {
            case Faction.Survivor:
                CheckPartyAutoAction(currentEntity);
                //Execute
                break;
            case Faction.Enemy:
                //Execute
                break;
            case Faction.neutral:
                //Execute
                break;
            default :
                break;
        }

        yield return null;
    }

    private bool CheckPartyAutoAction(BattleEntityBase entityForCheck)
    {
        PartyBattleEntity partyEntity;
        partyEntity = battleEntityManager.partyEntities.
            First(entity => entity == entityForCheck);
        if (partyEntity.isAutoExecuteAction)
        {
            return false;
            //AI ToDo
        }
        else
        {
            return false;
        }
    }
}
