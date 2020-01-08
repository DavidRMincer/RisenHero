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
    }
    
    /// <summary>
    /// Death
    /// </summary>
    public void Die()
    {
        if (captive)
        {
            //GameObject newCaptive = Instantiate(captive, new Vector2(captive.transform.position.x, captive.transform.position.y), Quaternion.identity);

            target.GetComponent<PlayerBehaviour>().AddCompanion(ref captive);
            Debug.Log("Before: " + captive.activeInHierarchy);
            //captive.SetActive(false);
            Debug.Log("After: " + captive.activeInHierarchy);
        }
        
        Destroy(gameObject);
    }
}
