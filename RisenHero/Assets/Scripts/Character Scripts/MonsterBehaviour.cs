using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : CharacterBehaviour
{
    public enum MonsterState
    {
        PASSIVE, FOLLOW, COMBAT
    };

    public GameObject       target,
                            captive;
    internal MonsterState   currentState;

    public override void Start()
    {
        base.Start();
        currentState = MonsterState.PASSIVE;
    }

    private void Update()
    {
        switch (currentState)
        {
            case MonsterState.PASSIVE:
                break;
            case MonsterState.FOLLOW:
                break;
            case MonsterState.COMBAT:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Die();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {

                }
                break;
            default:
                break;
        }

        if (GetHealth() <= 0)
        {
            Die();
        }
    }

    public void EngageCombat(GameObject challenger)
    {
        target = challenger;

        currentState = MonsterState.COMBAT;
    }

    public void Die()
    {
        if (captive)
        {
            target.GetComponent<PlayerBehaviour>().AddCompanion(captive);
        }
        
        Destroy(gameObject);
    }
}
