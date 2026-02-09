using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class BattleTurnManager : MonoBehaviour
{
    [SerializeField] BattleEntityManager battleEntityManager;
    [SerializeField] BattleVisualGUI battleVisualGUI;

    [SerializeField] public LogGUI logGUI;

    [SerializeField] public BattleState battleState;

    [SerializeField] private BattleStringDataSO battleTurnStringData;
    [SerializeField] private Color battleTurnLogColor = Color.yellow;

    private int currentTurn = 0;
    private bool isEndingTurn = false;

    private const int FIRST_PLAYER_ENTITY = 0;
    private const float PLYAER_END_TURN_DURATION = 0.8f;

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
        BattleVisualGUI.OnPlayerRequestEndTurn += HandlePlayerEndTurn;
    }

    private void Update()
    {
        switch (battleState)
        {
            case BattleState.Victory:
                ChangeState(BattleState.BattleEnd);
                SetBattleEnd();
                break;
            case BattleState.Defeat:
                ChangeState(BattleState.BattleEnd);
                SetBattleEnd();
                break;
            default:
                break;
        }
    }
    //----------------Battle Start Set Up---------------//
    private void SetUpBattleStart()
    {
        ChangeState(BattleState.Setup);

        for (int i = 0; i < battleEntityManager.allBattleEntities.Count; i++)
        {
            battleEntityManager.allBattleEntities[i].entityBattleState = BattleEntityActionStates.Attack;
        }

        battleEntityManager.currentPlayerEntity = FIRST_PLAYER_ENTITY;

        ChangeState(BattleState.PlayerTurnStart);
        currentTurn = 1;
        SetPlayerTurnStart();
    }

    //-------------------Battle End-------------------//
    private void SetBattleEnd()
    {
        SceneManager.LoadScene(SceneName.OVER_WORLD);
    }
    //----------------------Battle loop----------------------//
    //-------Player Turn Start--------/ 1 

    private void SetPlayerTurnStart()
    {
        isEndingTurn = false;
        battleVisualGUI.isPlayerCanExecuteAction = true;

        if (currentTurn != 1)
        {
            for (int i = 0; i < battleEntityManager.partyEntities.Count(); i++)
            {
                battleEntityManager.partyEntities.ElementAt(i).
                    EntityRecoverAP(battleEntityManager.partyEntities.ElementAt(i).eachTurnRecoveredAP);
            }
        }
        ChangeState(BattleState.PlayerTurnAction);
        SetPlayerTurnAtion();
    }
    //--------Player Turn Action--------/2          
    private void SetPlayerTurnAtion()
    {
        //Event
    }

    //--------Event
    private void HandlePlayerEndTurn()
    {
        if (isEndingTurn) return;
        if (battleState == BattleState.PlayerTurnAction)
        {
            ChangeState(BattleState.PlayerTurnAIAutoAction);
        }
        else
        {
            return;
        }
        isEndingTurn = true;
        StartCoroutine(SetPlayerAutoAction());
    }

    //------Player Turn AI Auto Action------/3
    private IEnumerator SetPlayerAutoAction()
    {
        yield return null;
        //目前不需要玩家角色的AI
        ChangeState(BattleState.PlayerTurnEnd);
        SetPlayerTurnEnd();
    }

    //----------Player Turn End----------/4
    private void SetPlayerTurnEnd()
    {
        battleVisualGUI.isPlayerCanExecuteAction = false;
        ChangeState(BattleState.EnemyTurnStart);
        SetEnemyTurnStart();
    }

    //---------Enemy Turn Start----------/5
    private void SetEnemyTurnStart()
    {
        if (currentTurn != 1)
        {
            for (int i = 0; i < battleEntityManager.enemyEntities.Count(); i++)
            {
                battleEntityManager.enemyEntities.ElementAt(i).
                    EntityRecoverAP(battleEntityManager.enemyEntities.ElementAt(i).eachTurnRecoveredAP);
            }
        }
        ChangeState(BattleState.EnemyTurnAction);
        StartCoroutine(SetEnemyAction());
    }
    //-----------Enemy Turn Action----------/6
    private IEnumerator SetEnemyAction()
    {
        yield return new WaitForSeconds(PLYAER_END_TURN_DURATION);
        yield return StartCoroutine(AutoBattle(Faction.Enemy));

        ChangeState(BattleState.EnemyTurnEnd);
        SetEnemyTurnEnd();
    }
    //------------Enemy Turn End----------/7
    private void SetEnemyTurnEnd()
    {
        CheckBattleVictoryOrDefeat();
        if (battleState == BattleState.BattleEnd) return;

        currentTurn++;
        ChangeState(BattleState.PlayerTurnStart);
        SetPlayerTurnStart();
    }

    //-----------------------Change State------------------------//
    private void ChangeState(BattleState newState)
    {
        battleState = newState;
        switch (newState)
        {
            case BattleState.Setup:
                break;
            case BattleState.PlayerTurnStart:
                UpdateBattleLog(battleTurnStringData.playerTurnStartText);
                break;
            case BattleState.PlayerTurnAction:
                break;
            case BattleState.PlayerTurnAIAutoAction:
                break;
            case BattleState.PlayerTurnEnd:
                UpdateBattleLog(battleTurnStringData.playerEndTurnText);
                break;
            case BattleState.EnemyTurnStart:
                break;
            case BattleState.EnemyTurnAction:
                break;
            case BattleState.EnemyTurnEnd:
                break;
            case BattleState.Victory:
                break;
            case BattleState.Defeat:
                break;
            case BattleState.Escape:
                break;
            case BattleState.BattleEnd:
                break;
            default:
                break;
        }
    }

    //----------------------Auto Battle(AI)------------------------//
    private IEnumerator AutoBattle(Faction faction)
    {
        for (int i = 0; i < battleEntityManager.allBattleEntities.Count; i++)
        {
            BattleEntityBase entity = battleEntityManager.allBattleEntities[i];

            if (entity.entityFaction == faction && !entity.EntityIsDead())
            {
                yield return StartCoroutine(AutoExecuteActionRoutine(entity, faction));
            }
        }

        yield return null;
    }

    private IEnumerator AutoExecuteActionRoutine(BattleEntityBase currentEntity, Faction faction)
    {
        switch (currentEntity.entityFaction)
        {
            case Faction.Survivor:
                CheckPartyAutoAction(currentEntity);//这里不会通过，因为玩家实体目前没有AI
                //Execute ToDo
                break;
            case Faction.Enemy:
                while (!currentEntity.EntityIsDead())
                {
                    DecisionAI decision = currentEntity.entityAI.GetDecisionAI(currentEntity, battleEntityManager);

                    if (decision.action != null && decision.action.CanExecute(currentEntity, decision.targets))
                    {
                        ActionBase action = AutoBattleExecuteAI(currentEntity, decision);
                        yield return new WaitForSeconds(action.actionDelay);
                    }
                    else { break; }
                }
                break;
            case Faction.neutral:
                //Execute
                break;
            default:
                break;
        }

        yield return null;
    }

    private ActionBase AutoBattleExecuteAI(BattleEntityBase currentEntity, DecisionAI decision)
    {
        currentEntity.ExecuteAction(decision.action, decision.targets);

        return decision.action;
    }

    private bool CheckPartyAutoAction(BattleEntityBase entityForCheck)
    {
        PartyBattleEntity partyEntity;
        partyEntity = battleEntityManager.partyEntities.
            First(entity => entity == entityForCheck);
        if (partyEntity.isAutoExecuteAction)
        {
            return false;
            //Player faction AI ToDo
        }
        else
        {
            return false;
        }
    }
    //---------------Log GUI-------------------//
    private void UpdateBattleLog(LocalizedString localizedString)
    {
        localizedString.GetLocalizedStringAsync().Completed += (op) =>
        {
            string msg = op.Result;
            logGUI.UpdateLog($">>> {msg} <<<", battleTurnLogColor); 
        };
    }


    //------------------Event------------------//
    public void CheckBattleVictoryOrDefeat()
    {
        if (!battleEntityManager.partyEntities.Any() && !battleEntityManager.enemyEntities.Any())
        {
            battleState = BattleState.Defeat; 
        }
        else if (!battleEntityManager.partyEntities.Any())
        {
            battleState = BattleState.Defeat;
        }
        else if (!battleEntityManager.enemyEntities.Any())
        {
            battleState = BattleState.Victory;
        }
    }
}
