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
    private Vector3             _camOffset;

    private void Start()
    {
        _overworldScript = overworldManager.GetComponent<OverworldGenerator>();
        _playerScript = player.GetComponent<PlayerBehaviour>();

        _camOffset = currentCam.transform.position;

        currentSegment = _overworldScript.GetSelected();
        player.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentSegment = _overworldScript.GetSelected();
            OpenSegment(_overworldScript.GetDirection());
        }
    }

    private void LateUpdate()
    {
        if (_inSegment)
        {
            currentCam.transform.position = player.transform.position + _camOffset;
        }
        else
        {
            currentCam.transform.position = currentSegment.transform.position + _camOffset;
        }
    }

    /// <summary>
    /// Load current segment
    /// </summary>
    /// <param name="direction"></param>
    public void OpenSegment(Vector2 direction)
    {
        Vector2 extraDistance = direction * (currentSegment.segSize / 2);
        extraDistance = (direction * ((currentSegment.segSize / 2) - (new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y) * 2))));
        Vector2 spawnPoint = currentSegment.GetCentre() + extraDistance;
        Debug.Log(currentSegment.GetCentre());

        _overworldScript.SetVisibility(false);
        overworldManager.SetActive(false);

        currentSegment.DrawSegment();
        player.GetComponent<Rigidbody2D>().transform.position = spawnPoint;

        _inSegment = true;
        player.SetActive(true);
        _playerScript.inputEnabled = true;
    }

    /// <summary>
    /// Close segment and move through overworld
    /// </summary>
    /// <param name="direction"></param>
    public void ExitSegment(Vector2 direction)
    {
        Debug.Log(direction);
        currentSegment.UnloadSegment();

        overworldManager.SetActive(true);
        _overworldScript.SetVisibility(true);

        _overworldScript.MoveSelection(direction);
        currentSegment = _overworldScript.GetSelected();

        _inSegment = false;
        _playerScript.inputEnabled = false;
        player.SetActive(false);
    }

    /// <summary>
    /// Returns centre point
    /// </summary>
    /// <returns></returns>
    public Vector2 GetSegCentre()
    {
        return currentSegment.GetCentre();
    }
}
