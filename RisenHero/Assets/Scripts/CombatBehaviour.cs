using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBehaviour : MonoBehaviour
{
    public enum CombatState
    {
        NONE, PLAYER_TURN, ALLY_TURN, ENEMY_TURN
    };

    public PlayerBehaviour          player;
    public List<CompanionBehaviour> allyTeam = new List<CompanionBehaviour>();
    public MonsterBehaviour         enemy;
    public float                    turnDuration;

    private CombatState             _currentState;
    private float                   _currentTurnDuration;
    private bool                    _turnInProgress = false,
                                    _inCombat = false;

    void Start()
    {
        _currentState = CombatState.NONE;
        _currentTurnDuration = turnDuration;
    }

    public void Setup(PlayerBehaviour p, MonsterBehaviour e)
    {
        // Set player and enemy
        player = p;
        enemy = e;

        e.target = p.gameObject;
        p.inputEnabled = false;

        // Add allies
        for (int i = 0; i < p.partyMembers.Count; i++)
        {
            AddAlly(p.partyMembers[i].GetComponent<CompanionBehaviour>());
        }

        if (e.captive)
        {
            AddAlly(e.captive.GetComponent<CompanionBehaviour>());
        }

        // Start turns
        _currentState = CombatState.PLAYER_TURN;
        _inCombat = true;
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

    void Update()
    {
        if (_inCombat)
        {
            switch (_currentState)
            {
                case CombatState.PLAYER_TURN:
                    PlayerTurn();
                    break;
                case CombatState.ALLY_TURN:
                    AllyTurn();
                    break;
                case CombatState.ENEMY_TURN:
                    EnemyTurn();
                    break;
                default:
                    break;
            }

            if (player.GetHealth() <= 0 ||
                enemy.GetHealth() <= 0)
            {
                EndCombat();
            }
        }
    }

    private void PlayerTurn()
    {
        if (Input.GetButtonDown("Action_1"))
        {
            player.actions[0].Perform(enemy);
            _currentState = CombatState.ALLY_TURN;
        }
    }

    private void AllyTurn()
    {

        for (int i = 0; i < allyTeam.Count; ++i)
        {
            ActionDelay(turnDuration);

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

        // End turn
        _currentState = CombatState.ENEMY_TURN;
        _turnInProgress = false;
    }

    private void EnemyTurn()
    {
        ActionDelay(turnDuration);

        enemy.actions[0].Perform(player);

        // End turn
        _currentState = CombatState.PLAYER_TURN;
        _turnInProgress = false;
    }

    public void ActionDelay(float duration)
    {
        _currentTurnDuration = duration;

        // Countdown
        do
        {
            _currentTurnDuration -= Time.deltaTime;
        } while (_currentTurnDuration > 0f);
    }

    public void EndCombat()
    {
        if (enemy.GetHealth() <= 0)
        {
            enemy.Die();
        }
        if (player.GetHealth() <= 0)
        {
            player.gameObject.SetActive(false);
        }

        allyTeam = new List<CompanionBehaviour>();

        player.inputEnabled = true;

        _currentState = CombatState.NONE;
        _inCombat = false;
    }
}
