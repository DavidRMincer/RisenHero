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

        LoadCheckpoint();
    }

    private void Update()
    {
        if (!_inSegment &&
            Input.GetButtonDown("Select"))
        {
            currentSegment = _overworldScript.GetSelected();
            OpenSegment(_overworldScript.GetDirection());
        }

        if (_inSegment &&
            Input.GetKeyDown(KeyCode.Return))
        {
            _playerScript.AddHealth(-_playerScript.maxHealth);
        }

        if (_playerScript.GetHealth() <= 0)
        {
            PlayerDeath(1);
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

        _overworldScript.SetVisibility(false);
        overworldManager.SetActive(false);

        currentSegment.DrawSegment();
        player.GetComponent<Rigidbody2D>().transform.position = spawnPoint;

        _inSegment = true;
        player.SetActive(true);
        _playerScript.inputEnabled = true;
        _playerScript.SpawnCompanions(direction * _overworldScript.tileSize);
    }

    /// <summary>
    /// Close segment and move through overworld
    /// </summary>
    /// <param name="direction"></param>
    public void ExitSegment(Vector2 direction)
    {
        currentSegment.UnloadSegment();

        overworldManager.SetActive(true);
        _overworldScript.SetVisibility(true);

        _overworldScript.MoveSelection(direction);
        currentSegment = _overworldScript.GetSelected();

        _inSegment = false;
        _playerScript.inputEnabled = false;
        _playerScript.DespawnCompanions();
        player.SetActive(false);
    }

    /// <summary>
    /// Open segment and spawn player at checkpoint
    /// </summary>
    public void LoadCheckpoint()
    {
        currentSegment = _overworldScript.GetCheckpoint();
        _overworldScript.SetSelectedToCheckpoint();

        _overworldScript.SetVisibility(false);
        overworldManager.SetActive(false);

        currentSegment.DrawSegment();

        // Position player next to checkpoint
        player.GetComponent<Rigidbody2D>().transform.position = (currentSegment.GetCheckpointTile() * currentSegment.tileSize * (Vector2.right + Vector2.down)) + (Vector2.right * currentSegment.tileSize);
        _playerScript.SetDirectionFacing(Vector2.left);

        // Position party members next to checkpoint
        _playerScript.SpawnCompanions(Vector2.left);

        for (int i = 0; i < _playerScript.partyMembers.Count; ++i)
        {
            if (i == 0)
            {
                _playerScript.partyMembers[i].GetComponent<Rigidbody2D>().transform.position = (currentSegment.GetCheckpointTile() * currentSegment.tileSize * (Vector2.right + Vector2.down)) + ((Vector2.left + Vector2.up) * currentSegment.tileSize);
            }
            else
            {
                _playerScript.partyMembers[i].GetComponent<Rigidbody2D>().transform.position = (currentSegment.GetCheckpointTile() * currentSegment.tileSize * (Vector2.right + Vector2.down)) + ((Vector2.left + Vector2.down) * currentSegment.tileSize);
            }

            _playerScript.partyMembers[i].GetComponent<CompanionBehaviour>().SetDirectionFacing(Vector2.right);
        }

        _inSegment = true;
        player.SetActive(true);
        _playerScript.inputEnabled = true;
    }

    /// <summary>
    /// Returns centre point
    /// </summary>
    /// <returns></returns>
    public Vector2 GetSegCentre()
    {
        return currentSegment.GetCentre();
    }

    /// <summary>
    /// Set overworld checkpoint tile as current segment
    /// </summary>
    public void SetCheckpointAsCurrent()
    {
        _overworldScript.SetCheckpoint(currentSegment.gameObject);
    }

    /// <summary>
    /// Player dies, loses party & world ages
    /// </summary>
    /// <param name="ageMultiplier"></param>
    public void PlayerDeath(int ageMultiplier)
    {
        // Close segment
        ExitSegment(Vector2.zero);

        // Age world
        _overworldScript.AgeWorld(ageMultiplier);

        // Destroy all companions
        for (int i = 0; i < _playerScript.partyMembers.Count; ++i)
        {
            if (_playerScript.partyMembers[i])
            {
                Destroy(_playerScript.partyMembers[i]);
            }
        }
        _playerScript.partyMembers = new List<GameObject>();

        // Restore health
        _playerScript.AddHealth(_playerScript.maxHealth);

        // Load checkpoint
        LoadCheckpoint();
    }
}
