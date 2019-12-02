using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject           overworldManager,
                                player;
    public Camera               currentCam;

    internal SegmentBehaviour   currentSegment;

    private OverworldGenerator  _overworldScript;
    private PlayerBehaviour     _playerScript;
    private bool                _inSegment = false;

    private void Start()
    {
        _overworldScript = overworldManager.GetComponent<OverworldGenerator>();
        _playerScript = player.GetComponent<PlayerBehaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentSegment = _overworldScript.GetSelected();
            OpenSegment(Vector2.zero);
        }
    }
    
    /// <summary>
    /// Load current segment
    /// </summary>
    /// <param name="direction"></param>
    public void OpenSegment(Vector2 direction)
    {
        Vector2 spawnPoint = currentSegment.GetCentre() - (direction * ((currentSegment.segSize / 2) - direction));

        currentSegment.DrawSegment();
        player.GetComponent<Rigidbody2D>().transform.position = spawnPoint;

        _inSegment = true;
    }
}
