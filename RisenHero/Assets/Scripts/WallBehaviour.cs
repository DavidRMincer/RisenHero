using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    private GameManagerBehaviour _gm;

    private void Start()
    {
        _gm = FindObjectOfType<GameManagerBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("TRIGGERED!");
            StartCoroutine(_gm.CheckpointDeath());
        }
    }
}
