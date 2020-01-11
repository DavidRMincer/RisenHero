using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerBehaviour : MonoBehaviour
{
    public GameObject           overworldManager,
                                player,
                                grassTile,
                                leafParticles;
    public Camera               currentCam;
    public List<GameObject>     companionTypes,
                                startingMonster;
    public float                campfireDuration,
                                fadeToBlackDuration,
                                fadeToWhiteDuration,
                                fadeInDuration,
                                deathDuration;
    public Vector2              minOverworldCamPos,
                                maxOverworldCamPos;
    public UIManagerBehaviour   uiManager;
    public string               mainMenuScene;

    internal SegmentBehaviour   currentSegment;

    private OverworldGenerator  _overworldScript;
    private PlayerBehaviour     _playerScript;
    private bool                _inSegment = false,
                                _dying = false,
                                _paused = false,
                                _aged = false;
    private Vector3             _camOffset;
    private Vector2             _minSegmentCamPos = -(Vector2.one / 2),
                                _maxSegmentCamPos = Vector2.one / 2;


    private void Start()
    {
        _overworldScript = overworldManager.GetComponent<OverworldGenerator>();
        _playerScript = player.GetComponent<PlayerBehaviour>();

        _camOffset = currentCam.transform.position.z * Vector3.forward;

        LoadCheckpoint();

        // Firstmonster and captive spawned
        startingMonster[0].transform.position = new Vector2(currentSegment.segSize.x * 0.75f, currentSegment.GetCheckpointTile().y) * (Vector2.right + Vector2.down) * currentSegment.tileSize;
        startingMonster[1].transform.position = new Vector2(startingMonster[0].transform.position.x, startingMonster[0].transform.position.y) + (Vector2.right * currentSegment.tileSize);

        uiManager.SetupHearts(_playerScript.maxHealth);

        StartCoroutine(DisableInputForDuration(fadeInDuration + 0.3f));
        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeInDuration));
    }

    private void Update()
    {
        if (!_inSegment &&
            Input.GetButtonDown("Select"))
        {
            currentSegment = _overworldScript.GetSelected();
            StartCoroutine(OpenSegment(_overworldScript.GetDirection()));
        }

        if (_inSegment &&
            Input.GetKeyDown(KeyCode.Return))
        {
            _playerScript.AddHealth(-_playerScript.maxHealth);
        }

        //if (!_dying &&
        //    _playerScript.GetHealth() <= 0)
        //{
        //    StartCoroutine(PlayerDeath(1));
        //}
    }

    private void LateUpdate()
    {
        float height = Mathf.Abs(currentCam.ScreenToWorldPoint(new Vector2(0, 0)).y - currentCam.transform.position.y);
        float width = Mathf.Abs(currentCam.ScreenToWorldPoint(new Vector2(0, 0)).x - currentCam.transform.position.x);

        grassTile.SetActive(_inSegment);
        leafParticles.SetActive(_inSegment);
        uiManager.healthBar.SetActive(_inSegment);

        if (_inSegment)
        {
            if (!_paused &&
                Input.GetButtonDown("Pause"))
            {
                uiManager.pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }

            currentCam.transform.position = player.transform.position + _camOffset;

            Vector3 newCamPos = currentCam.transform.position;

            newCamPos.x = Mathf.Clamp(newCamPos.x, _minSegmentCamPos.x + width + (currentSegment.tileSize * 0.25f), _maxSegmentCamPos.x - width - (currentSegment.tileSize * 0.75f));
            newCamPos.y = Mathf.Clamp(newCamPos.y, _minSegmentCamPos.y + height + (currentSegment.tileSize * 1f), _maxSegmentCamPos.y - height);

            currentCam.transform.position = newCamPos;

            _playerScript.rend.sortingOrder = Mathf.Abs(Mathf.CeilToInt(-player.transform.position.y) / currentSegment.tileSize) + 1;

            for (int i = 0; i < _playerScript.partyMembers.Count; ++i)
            {
                _playerScript.partyMembers[i].GetComponent<CompanionBehaviour>().rend.sortingOrder = Mathf.Abs(Mathf.CeilToInt(_playerScript.partyMembers[i].transform.position.y) / currentSegment.tileSize) + 1;
            }
        }
        else
        {
            currentCam.transform.position = currentSegment.transform.position + _camOffset;

            Vector3 newCamPos = currentCam.transform.position;

            newCamPos.x = Mathf.Clamp(newCamPos.x, minOverworldCamPos.x + width, maxOverworldCamPos.x - width);
            newCamPos.y = Mathf.Clamp(newCamPos.y, minOverworldCamPos.y + height, maxOverworldCamPos.y - height);

            currentCam.transform.position = newCamPos;

            currentCam.transform.position = newCamPos;
        }
    }

    /// <summary>
    /// Load current segment
    /// </summary>
    /// <param name="direction"></param>
    public IEnumerator OpenSegment(Vector2 direction)
    {
        StartCoroutine(uiManager.FadeTo(uiManager.blackout, fadeToBlackDuration));
        yield return new WaitForSeconds(fadeToBlackDuration + 0.5f);

        Vector2 extraDistance = direction * (currentSegment.segSize / 2);
        extraDistance = (direction * ((currentSegment.segSize / 2) - (new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y) * 2))));
        Vector2 spawnPoint = currentSegment.GetCentre() + extraDistance;

        _overworldScript.SetVisibility(false);
        overworldManager.SetActive(false);

        DrawSegment();
        player.GetComponent<Rigidbody2D>().transform.position = spawnPoint;

        _minSegmentCamPos.y = -(currentSegment.segSize.y * currentSegment.tileSize);
        _maxSegmentCamPos.x = currentSegment.segSize.x * currentSegment.tileSize;

        _inSegment = true;
        player.SetActive(true);
        _playerScript.SpawnCompanions(direction * _overworldScript.tileSize);

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToBlackDuration));
        yield return new WaitForSeconds(fadeToBlackDuration + 0.2f);

        _playerScript.inputEnabled = true;
    }

    /// <summary>
    /// Close segment and move through overworld
    /// </summary>
    /// <param name="direction"></param>
    public IEnumerator ExitSegment(Vector2 direction)
    {
        _playerScript.inputEnabled = false;

        StartCoroutine(uiManager.FadeTo(uiManager.blackout, fadeToBlackDuration));
        yield return new WaitForSeconds(fadeToBlackDuration + 0.5f);

        currentSegment.UnloadSegment();

        overworldManager.SetActive(true);
        _overworldScript.SetVisibility(true);

        _overworldScript.MoveSelection(direction);
        currentSegment = _overworldScript.GetSelected();

        _inSegment = false;
        _playerScript.DespawnCompanions();
        player.SetActive(false);

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToBlackDuration));
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

        _minSegmentCamPos.y = -(currentSegment.segSize.y * currentSegment.tileSize);
        _maxSegmentCamPos.x = currentSegment.segSize.x * currentSegment.tileSize;

        DrawSegment();

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
    public IEnumerator SetCheckpointAsCurrent()
    {
        _playerScript.inputEnabled = false;

        StartCoroutine(uiManager.FadeTo(uiManager.blackout, fadeToBlackDuration));

        yield return new WaitForSeconds(fadeToBlackDuration + 0.2f);
        
        uiManager.actionInputImg.gameObject.SetActive(false);

        _overworldScript.SetCheckpoint(currentSegment.gameObject);

        player.transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.right * currentSegment.tileSize);
        _playerScript.rend.flipX = true;

        for (int i = 0; i < _playerScript.partyMembers.Count; ++i)
        {
            if (i == 0)
            {
                _playerScript.partyMembers[i].transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.left + Vector2.up * currentSegment.tileSize);
            }
            else
            {
                _playerScript.partyMembers[i].transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.left + Vector2.down * currentSegment.tileSize);
            }
            _playerScript.partyMembers[i].GetComponent<CompanionBehaviour>().rend.flipX = false;
        }

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToBlackDuration));
        yield return new WaitForSeconds(fadeToBlackDuration + (campfireDuration / 2));

        while (_playerScript.GetHealth() < _playerScript.maxHealth)
        {
            _playerScript.AddHealth(1);
            uiManager.UpdateHealth(_playerScript.GetHealth());

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(campfireDuration / 2);

        StartCoroutine(uiManager.FadeTo(uiManager.blackout, fadeToBlackDuration));
        yield return new WaitForSeconds(fadeToBlackDuration + 0.2f);
        
        uiManager.actionInputImg.gameObject.SetActive(false);

        _playerScript.rend.flipX = true;

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToBlackDuration));

        yield return new WaitForSeconds(fadeToBlackDuration + 0.2f);

        _playerScript.inputEnabled = true;
        uiManager.actionInputImg.gameObject.SetActive(true);
    }

    public IEnumerator CheckpointDeath()
    {
        _playerScript.inputEnabled = false;

        StartCoroutine(uiManager.FadeTo(uiManager.blackout, fadeToBlackDuration));

        yield return new WaitForSeconds(fadeToBlackDuration + 0.2f);
        
        uiManager.actionInputImg.gameObject.SetActive(false);

        _overworldScript.SetCheckpoint(currentSegment.gameObject);

        player.transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.right * currentSegment.tileSize);
        _playerScript.rend.flipX = true;

        for (int i = 0; i < _playerScript.partyMembers.Count; ++i)
        {
            if (i == 0)
            {
                _playerScript.partyMembers[i].transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.left + Vector2.up * currentSegment.tileSize);
            }
            else
            {
                _playerScript.partyMembers[i].transform.position = (currentSegment.GetCheckpointTile() * (Vector2.down + Vector2.right)) + (Vector2.left + Vector2.down * currentSegment.tileSize);
            }
            _playerScript.partyMembers[i].GetComponent<CompanionBehaviour>().rend.flipX = false;
        }

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToBlackDuration));

        yield return new WaitForSeconds(fadeToBlackDuration + (campfireDuration / 2));

        while (_playerScript.GetHealth() > 0)
        {
            _playerScript.AddHealth(-1);
            uiManager.UpdateHealth(_playerScript.GetHealth());

            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(PlayerDeath(_overworldScript.GetCheckpointPos().x > _overworldScript.worldSize.x * 0.75f ?
            _overworldScript.GetDeadline() - 1 - _overworldScript.GetTimePeriod() :
            1));
    }

    /// <summary>
    /// Player dies, loses party & world ages
    /// </summary>
    /// <param name="ageMultiplier"></param>
    public IEnumerator PlayerDeath(int ageMultiplier)
    {
        _dying = true;
        _aged = false;
        _playerScript.inputEnabled = false;

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(uiManager.FadeTo(uiManager.whiteout, fadeToWhiteDuration));
        yield return new WaitForSeconds(fadeToWhiteDuration);

        yield return new WaitForSeconds(deathDuration);

        // Close segment
        currentSegment.UnloadSegment();

        if (_overworldScript.GetTimePeriod() + ageMultiplier >= _overworldScript.GetDeadline())
        {
            SceneManager.LoadScene("Game Over");
        }

        // Age world
        if (!_aged)
        {
            _overworldScript.AgeWorld(ageMultiplier);
            _aged = true;
        }

        uiManager.countdownText.text = _overworldScript.GetYearsRemaining().ToString() + " Years Left";
        uiManager.countdownText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        uiManager.countdownText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

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
        uiManager.UpdateHealth(_playerScript.GetHealth());

        // Load checkpoint
        LoadCheckpoint();
        _playerScript.inputEnabled = false;

        StartCoroutine(uiManager.FadeTo(uiManager.transparent, fadeToWhiteDuration));
        yield return new WaitForSeconds(fadeToWhiteDuration + 0.2f);

        Debug.Log("Time period: " + _overworldScript.GetTimePeriod());

        _playerScript.inputEnabled = true;
        _dying = false;
    }

    /// <summary>
    /// Draw current segment
    /// </summary>
    public void DrawSegment()
    {
        currentSegment.DrawSegment();

        if (_overworldScript.GetTimePeriod() < _overworldScript.GetDeadline() - 1 &&
            _playerScript.partyMembers.Count < 2)
        {
            currentSegment.AssignCaptive(companionTypes[Random.Range(0, companionTypes.Count - 1)]);
        }
    }

    /// <summary>
    /// Disable input while counting down
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator DisableInputForDuration(float duration)
    {
        _playerScript.inputEnabled = false;

        yield return new WaitForSeconds(duration);

        _playerScript.inputEnabled = true;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
        uiManager.pauseMenu.SetActive(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }
}
