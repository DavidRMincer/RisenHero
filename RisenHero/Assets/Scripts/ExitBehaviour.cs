using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehaviour : MonoBehaviour
{
    private GameManagerBehaviour    _gameManager;
    private Vector2                 _direction;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehaviour>();
        _direction = (new Vector2(transform.position.x, transform.position.y) - _gameManager.GetSegCentre()).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(_gameManager.ExitSegment(_direction));
        }
    }
}
