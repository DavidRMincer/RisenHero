using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2              worldSize;
    public float                tileSize,
                                selectionPulseSpeed;
    public GameObject           wallTile,
                                forestTile,
                                villageTile,
                                townTile,
                                castleTile,
                                ruinsTile,
                                startTile,
                                endTile;
    public int                  numofVillages,
                                numofTowns,
                                numofCastles,
                                numofRuins,
                                minGap,
                                maxGap,
                                ruinUpgradeChance,
                                villageUpgradeChance,
                                villageRuinChance,
                                townRuinChance,
                                castleRuinChance,
                                maxTimePeriod;
    public AnimationCurve       selectedScale;
    
    GameObject[,]               _world;
    private Vector2             _startPoint,
                                _endPoint,
                                _selectedTile,
                                _tileScale;
    private List<GameObject>    _segmentstoSpawn = new List<GameObject>();
    private int                 _timePeriod = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Fill segments list
        for (int i = numofCastles; i > 0; --i)
        {
            _segmentstoSpawn.Add(castleTile);
        }
        for (int i = numofRuins; i > 0; --i)
        {
            _segmentstoSpawn.Add(ruinsTile);
        }
        for (int i = numofTowns; i > 0; --i)
        {
            _segmentstoSpawn.Add(townTile);
        }
        for (int i = numofVillages; i > 0; --i)
        {
            _segmentstoSpawn.Add(villageTile);
        }

        GenerateWorld();

        for (int x = 0; x < _world.GetLength(0); ++x)
        {
            for (int y = 0; y < _world.GetLength(1); ++y)
            {
                if (_world[x, y])
                {
                    //Set tile as invisible
                    if (x != Mathf.RoundToInt(_startPoint.x) ||
                        y != Mathf.RoundToInt(_startPoint.y))
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.INVISIBLE);
                    }
                    
                    // Add entrances/exits
                    if (x > 0 &&
                        _world[x - 1, y])
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>()._westEntrance = true;
                    }
                    if (x < worldSize.x - 1 &&
                        _world[x + 1, y])
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>()._eastEntrance = true;
                    }
                    if (y > 0 &&
                        _world[x, y - 1])
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>()._northEntrance = true;
                    }
                    if (y < worldSize.y - 1 &&
                        _world[x, y + 1])
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>()._southEntrance = true;
                    }

                    // Generate each segment
                    _world[x, y].GetComponent<SegmentBehaviour>().GenerateSegment();
                }
            }
        }

        _tileScale = _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y)].transform.localScale;
        _selectedTile = _startPoint;
    }

    private void Update()
    {
        // For debugging only
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AgeWorld(1);
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().DrawSegment();
        //    gameObject.SetActive(false);
        //}

        // Change selected tile
        if (Input.GetKeyDown(KeyCode.LeftArrow) &&
            _selectedTile.x > 0)
        {
            MoveSelection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) &&
            _selectedTile.x < worldSize.x - 1)
        {
            MoveSelection(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) &&
            _selectedTile.y > 0)
        {
            MoveSelection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) &&
            _selectedTile.y < worldSize.y - 1)
        {
            MoveSelection(Vector2.down);
        }

        // Scale selected
        Vector2 newScale;

        newScale.x = selectedScale.Evaluate(Time.time * selectionPulseSpeed);
        newScale.y = selectedScale.Evaluate(Time.time * selectionPulseSpeed);

        _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].transform.localScale = newScale * _tileScale;
    }

    /// <summary>
    /// Procedurally generate overworld
    /// </summary>
    public void GenerateWorld()
    {
        _world = new GameObject[Mathf.RoundToInt(worldSize.x), Mathf.RoundToInt(worldSize.y)];

        // Set start point
        _startPoint.x = 0;
        _startPoint.y = Mathf.RoundToInt(worldSize.y / 2);
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y)] = GameObject.Instantiate(startTile, _startPoint * tileSize, Quaternion.identity);

        // Add path below start point
        // Through forest to village
        _world[Mathf.RoundToInt(_startPoint.x) + 1, Mathf.RoundToInt(_startPoint.y)] = GameObject.Instantiate(forestTile, (_startPoint + Vector2.right) * tileSize, Quaternion.identity);
        _world[Mathf.RoundToInt(_startPoint.x) + 2, Mathf.RoundToInt(_startPoint.y)] = GameObject.Instantiate(forestTile, (_startPoint + (Vector2.right * 2)) * tileSize, Quaternion.identity);
        _world[Mathf.RoundToInt(_startPoint.x) + 3, Mathf.RoundToInt(_startPoint.y)] = GameObject.Instantiate(villageTile, (_startPoint + (Vector2.right * 3)) * tileSize, Quaternion.identity);

        // Generate branching path
        BranchPath(Mathf.RoundToInt(_startPoint.x) + 3, Mathf.RoundToInt(_startPoint.y), true);
    }

    /// <summary>
    /// Generate procedural path
    /// </summary>
    /// <param name="segments"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="major"></param>
    private void BranchPath(int x, int y, bool major)
    {
        bool complete = false;
        int xIndex = x,
            yIndex = y,
            gapCounter = Random.Range(minGap, maxGap);

        // Generate path
        while (!complete)
        {
            int randomDir = Random.Range(0, 3),
                xDir = 0,
                yDir = 0;

            if (randomDir == 0)
            {
                xDir = 1;
            }
            else
            {
                yDir = randomDir == 1 ? 1 : -1;
            }
            
            // Reached end
            if (major &&
                xIndex == worldSize.x - 1)
            {
                complete = true;
            }
            else if ((yIndex + yDir >= 0 &&
                yIndex + yDir < worldSize.y) &&

                _world[xIndex + xDir, yIndex + yDir] == null)
            {
                xIndex += xDir;
                yIndex += yDir;

                // Add final wall
                if (xIndex == Mathf.RoundToInt(worldSize.x - 2))
                {
                    ReplaceTile(xIndex, yIndex, wallTile);

                    ++xIndex;

                    // Add end tile
                    _endPoint.x = xIndex;
                    _endPoint.y = yIndex;

                    _world[xIndex, yIndex] = GameObject.Instantiate(endTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);

                }
                // Add wall
                else if (xIndex == Mathf.RoundToInt(worldSize.x / 3) ||
                    xIndex == Mathf.RoundToInt(worldSize.x / 3) * 2)
                {
                    ReplaceTile(xIndex, yIndex, wallTile);

                    ++xIndex;
                    _world[xIndex, yIndex] = GameObject.Instantiate(forestTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);
                }
                // Spawn random segment
                else if (gapCounter == 0)
                {
                    _world[xIndex, yIndex] = GameObject.Instantiate(_segmentstoSpawn[Random.Range(0, _segmentstoSpawn.Count - 1)], new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity); ;

                    gapCounter = Random.Range(minGap, maxGap);
                }
                // Spawn forest tile
                else
                {
                    _world[xIndex, yIndex] = GameObject.Instantiate(forestTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);

                    --gapCounter;
                }
            }
        }
    }

    /// <summary>
    /// Destroy old tile and instantiate new tile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="newTile"></param>
    private void ReplaceTile(int x, int y, GameObject newTile)
    {
        Destroy(_world[x, y]);
        _world[x, y] = GameObject.Instantiate(newTile, new Vector2(x, y) * tileSize, Quaternion.identity);
    }

    /// <summary>
    /// Update world into future by (50 x multiplier) years
    /// </summary>
    /// <param name="multiplier"></param>
    public void AgeWorld(int multiplier)
    {
        // Update time period
        _timePeriod += multiplier;

        // For each segment
        for (int xIndex = 0; xIndex < worldSize.x; ++xIndex)
        {
            for (int yIndex = 0; yIndex < worldSize.y; ++yIndex)
            {
                if (_world[xIndex, yIndex] != null)
                {
                    // Update ruins to village
                    if ((       xIndex != Mathf.RoundToInt(worldSize.x / 3) && xIndex != Mathf.RoundToInt(worldSize.x / 3) * 2 && xIndex != Mathf.RoundToInt(worldSize.x - 2)) &&
                                _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.RUINS &&
                                Random.Range(0, 100) <= ruinUpgradeChance * multiplier)
                    {
                        ReplaceTile(xIndex, yIndex, villageTile);
                    }
                    // Update village to town
                    else if (   _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.VILLAGE &&
                                Random.Range(0, 100) <= villageUpgradeChance * multiplier)
                    {
                        ReplaceTile(xIndex, yIndex, townTile);
                    }
                    // Update to ruins
                    else if (   (_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.VILLAGE &&
                                Random.Range(0, 100) <= villageRuinChance * multiplier) ||
                                (_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.TOWN &&
                                Random.Range(0, 100) <= townRuinChance * multiplier) ||
                                (_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.CASTLE &&
                                Random.Range(0, 100) <= castleRuinChance * multiplier) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x / 3) && _timePeriod >= 1) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x / 3) * 2 && _timePeriod >= 2) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x - 2) && _timePeriod == maxTimePeriod - 1))
                    {
                        ReplaceTile(xIndex, yIndex, ruinsTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns script of currently selected segment
    /// </summary>
    /// <returns></returns>
    public SegmentBehaviour GetSelected()
    {
        return _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>();
    }

    /// <summary>
    /// Set visibility of tiles
    /// </summary>
    /// <param name="visible"></param>
    public void SetVisibility(bool visible)
    {
        for (int x = 0; x < Mathf.RoundToInt(worldSize.x); ++x)
        {
            for (int y = 0; y < Mathf.RoundToInt(worldSize.y); ++y)
            {
                if (_world[x, y])
                {
                    _world[x, y].SetActive(visible);
                }
            }
        }
    }

    /// <summary>
    /// Moves tile selection
    /// </summary>
    /// <param name="direction"></param>
    public void MoveSelection(Vector2 direction)
    {
        _selectedTile += direction;

        if (_world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().displayType != SegmentBehaviour.DisplayCategory.VISIBLE)
        {
            _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.VISIBLE);
        }
    }
}
