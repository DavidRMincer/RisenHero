using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    public GameObject               flameParticles;

    internal int                    tileSize;

    private GameManagerBehaviour    _gm;
    private UIManagerBehaviour      _uiM;

    private void Start()
    {
        _gm = FindObjectOfType<GameManagerBehaviour>();
        _uiM = _gm.uiManager;

        flameParticles.SetActive(true);
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Press action_1 to set checkpoint
            if (Input.GetButtonDown("Action_1"))
            {
                StartCoroutine(_gm.SetCheckpointAsCurrent());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _uiM.actionInputImg.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _uiM.actionInputImg.gameObject.SetActive(false);
    }
}
