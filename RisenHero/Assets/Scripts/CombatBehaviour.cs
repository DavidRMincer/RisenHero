using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehaviour : MonoBehaviour
{
    public enum CombatState
    {
        NONE, ALLY_TURN, ENEMY_TURN
    };

    public PlayerBehaviour          player;
    public List<CompanionBehaviour> allyTeam = new List<CompanionBehaviour>();
    public MonsterBehaviour         enemy;
    public float                    turnDuration;

    private CombatState             _currentState;
    private float                   _currentTurnDuration;
    private bool                    _turnInProgress = false;

    private void Start()
    {
        _currentState = CombatState.NONE;
        _currentTurnDuration = turnDuration;
    }

    public void AddPlayer(PlayerBehaviour p)
    {
        player = p;
        player.inputEnabled = false;
    }

    public void AddAlly(CompanionBehaviour c)
    {
        allyTeam.Add(c);
    }

    public void AddEnemy(MonsterBehaviour e)
    {
        enemy = e;
    }

    IEnumerator PlayAllyTurn()
    {
        // Countdown
        do
        {
            _currentTurnDuration -= Time.deltaTime;
        } while (_currentTurnDuration > 0f);

        for (int i = 0; i < allyTeam.Count; ++i)
        {
            // Countdown
            do
            {
                _currentTurnDuration -= Time.deltaTime;
            } while (_currentTurnDuration > 0f);

            int index = Random.Range(0, allyTeam[i].actions.Count - 1);

            switch (allyTeam[i].actions[index].type)
            {
                case ActionBehaviour.ActionType.ATTACK:
                    allyTeam[i].actions[index].Perform(enemy);
                    break;
                case ActionBehaviour.ActionType.HEAL:
                    allyTeam[i].actions[index].Perform(player);
                    break;
                default:
                    break;
            }

        }

        _currentState = CombatState.ENEMY_TURN;
        _turnInProgress = false;
        yield return null;
    }

    public void PlayTurn()
    {
        switch (_currentState)
        {
            case CombatState.ALLY_TURN:
                if (!_turnInProgress)
                {
                    StartCoroutine("PlayAllyTurn");
                    _turnInProgress = true;
                }
                break;
            case CombatState.ENEMY_TURN:
                break;
            default:
                break;
        }
    }

    public void EndCombat()
    {
        allyTeam = new List<CompanionBehaviour>();
        enemy = null;
    }
}
