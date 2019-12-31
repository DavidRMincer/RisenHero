using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    internal int                    tileSize;

    private GameManagerBehaviour    _gm;

    private void Start()
    {
        _gm = FindObjectOfType<GameManagerBehaviour>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Press action_1 to set checkpoint
            if (Input.GetButtonDown("Action_1"))
            {
                Debug.Log("TRIGGERED!");
                _gm.SetCheckpointAsCurrent();
            }
        }
    }
}
