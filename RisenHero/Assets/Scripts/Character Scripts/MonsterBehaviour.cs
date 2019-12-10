using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    public enum MonsterState
    {
        PASSIVE, FOLLOW, COMBAT
    };

    public GameObject       target,
                            captive;
    internal MonsterState   currentState;

    private void Start()
    {
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
    }

    public void EngageCombat(GameObject challenger)
    {
        target = challenger;

        currentState = MonsterState.COMBAT;
    }

    public void Die()
    {
        target.GetComponent<PlayerBehaviour>().AddCompanion(captive);
        Destroy(gameObject);
    }
}
