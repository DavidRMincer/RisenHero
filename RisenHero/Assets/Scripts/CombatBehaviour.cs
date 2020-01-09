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
    public UIManagerBehaviour       UIManager;

    private CombatState             _currentState;
    private float                   _currentTurnDuration;
    private bool                    _turnInProgress = false,
                                    _inCombat = false;
    private GameManagerBehaviour    _gm;

    void Start()
    {
        _currentState = CombatState.NONE;
        _currentTurnDuration = turnDuration;
        _gm = FindObjectOfType<GameManagerBehaviour>();
    }

    public void Setup(PlayerBehaviour p, MonsterBehaviour e)
    {
        // Set player and enemy
        player = p;
        enemy = e;

        e.target = p.gameObject;
        p.inputEnabled = false;

        p.ReplenishCooldown();
        e.ReplenishCooldown();

        // Add allies
        for (int i = 0; i < p.partyMembers.Count; i++)
        {
            AddAlly(p.partyMembers[i].GetComponent<CompanionBehaviour>());
            p.partyMembers[i].GetComponent<CompanionBehaviour>().ReplenishCooldown();
        }

        if (e.captive)
        {
            AddAlly(e.captive.GetComponent<CompanionBehaviour>());
            e.captive.GetComponent<CompanionBehaviour>().ReplenishCooldown();
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

            UIManager.UpdateHealth(player.GetHealth());
        }
    }

    private void PlayerTurn()
    {
        ResetTurnTimer(turnDuration);

        if (_currentTurnDuration <= 0f)
        {
            UIManager.actionInputImg.gameObject.SetActive(true);

            if (Input.GetButtonDown("Action_1"))
            {
                player.actions[0].Perform(enemy);
                _turnInProgress = false;
                _currentState = CombatState.ALLY_TURN;

                UIManager.actionInputImg.gameObject.SetActive(false);
            }
        }
        // Countdown
        else
        {
            _currentTurnDuration -= Time.deltaTime;
        }
    }

    private void AllyTurn()
    {
        ResetTurnTimer(turnDuration);

        if (_currentTurnDuration <= 0f)
        {
            for (int i = 0; i < allyTeam.Count; ++i)
            {
                ResetTurnTimer(turnDuration);

                int index = Random.Range(0, allyTeam[i].actions.Count - 1);
                
                switch (allyTeam[i].actions[index].type)
                {
                    case ActionBehaviour.ActionType.ATTACK:
                        allyTeam[i].actions[index].Perform(enemy);
                        break;
                    case ActionBehaviour.ActionType.HEAVY_ATTACK:
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
        // Countdown
        else
        {
            _currentTurnDuration -= Time.deltaTime;
        }
    }

    private void EnemyTurn()
    {
        ResetTurnTimer(turnDuration);

        if (_currentTurnDuration <= 0f)
        {
            enemy.actions[0].Perform(player);

            // End turn
            _currentState = CombatState.PLAYER_TURN;
            _turnInProgress = false;
        }
        // Countdown
        else
        {
            _currentTurnDuration -= Time.deltaTime;
        }
    }

    public void ResetTurnTimer(float duration)
    {
        if (!_turnInProgress)
        {
            _currentTurnDuration = duration;
            _turnInProgress = true;
        }
    }

    public void EndCombat()
    {
        if (enemy)
        {
            enemy.Die();
        }

        allyTeam = new List<CompanionBehaviour>();

        player.inputEnabled = true;

        //if (_gm.startingMonster[1])
        //{
        //    _gm.startingMonster.Remove(_gm.startingMonster[1]);
        //}
        //_gm.currentSegment.RemoveCaptive();

        _currentState = CombatState.NONE;
        _inCombat = false;
    }
}
