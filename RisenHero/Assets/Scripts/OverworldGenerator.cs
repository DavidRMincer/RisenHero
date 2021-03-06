﻿using System.Collections;
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
                                ruinsTile,
                                startTile,
                                endTile,
                                smallMonster,
                                mediumMonster,
                                largeMonster;
    public int                  numofVillages,
                                numofTowns,
                                numofRuins,
                                minGap,
                                maxGap,
                                ruinUpgradeChance,
                                villageUpgradeChance,
                                villageRuinChance,
                                townRuinChance,
                                maxTimePeriod;
    public AnimationCurve       selectedScale;

    private GameObject[,]       _world;
    private Vector2             _startPoint,
                                _endPoint,
                                _selectedTile,
                                _tileScale,
                                _direction = Vector2.zero,
                                _checkpointSegment;
    private List<GameObject>    _segmentstoSpawn = new List<GameObject>();
    private int                 _timePeriod = 0;

    // Start is called before the first frame update
    void Awake()
    {
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
                    // Set start tile as visible
                    //if (x == Mathf.RoundToInt(_startPoint.x) &&
                    //    y == Mathf.RoundToInt(_startPoint.y))
                    //{
                    //    _world[x, y].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.VISIBLE);
                    //}
                    //else if (   x == Mathf.RoundToInt(_startPoint.x) + 1 &&
                    //            y == Mathf.RoundToInt(_startPoint.y))
                    //{
                    //    _world[x, y].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.PREVIEW);
                    //}
                    ////Set tile as invisible
                    //else
                    //{
                    //    _world[x, y].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.INVISIBLE);
                    //}

                    _world[x, y].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.INVISIBLE);

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
                        _world[x, y].GetComponent<SegmentBehaviour>()._southEntrance = true;
                    }
                    if (y < worldSize.y - 1 &&
                        _world[x, y + 1])
                    {
                        _world[x, y].GetComponent<SegmentBehaviour>()._northEntrance = true;
                    }

                    // Generate each segment
                    _world[x, y].GetComponent<SegmentBehaviour>().GenerateSegment();
                }
            }
        }

        _tileScale = _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y)].transform.localScale;
        _selectedTile = _startPoint;

        MoveSelection(Vector2.zero);
    }

    private void Update()
    {
        // For debugging only
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    AgeWorld(1);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().DrawSegment();
        //    gameObject.SetActive(false);
        //}

        // Change selected tile
        //if (Input.GetKeyDown(KeyCode.LeftArrow) &&
        //    _selectedTile.x > 0)
        //{
        //    MoveSelection(Vector2.left);
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow) &&
        //    _selectedTile.x < worldSize.x - 1)
        //{
        //    MoveSelection(Vector2.right);
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow) &&
        //    _selectedTile.y > 0)
        //{
        //    MoveSelection(Vector2.down);
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow) &&
        //    _selectedTile.y < worldSize.y - 1)
        //{
        //    MoveSelection(Vector2.up);
        //}

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

        // Generate branching path
        BranchPath(Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y));

        // Set first checkpoint
        _checkpointSegment = _startPoint;
        
    }

    /// <summary>
    /// Generate procedural path
    /// </summary>
    /// <param name="segments"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="major"></param>
    private void BranchPath(int x, int y)
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

            if (xIndex == _startPoint.x ||
                randomDir == 0)
            {
                xDir = 1;
            }
            else
            {
                yDir = randomDir == 1 ? 1 : -1;
            }
            
            // Reached end
            if (xIndex == worldSize.x - 1)
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

                    // Add monsters to list of monsters
                    if (xIndex > (worldSize.x / 3) * 2)
                    {
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters = new List<GameObject>(3)
                        {
                            largeMonster,
                            mediumMonster,
                            smallMonster
                        };
                        //Debug.Log(_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters.Count + "/" + 3);
                    }
                    else if (xIndex > worldSize.x / 3)
                    {
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters = new List<GameObject>(2)
                        {
                            mediumMonster,
                            smallMonster
                        };
                        //Debug.Log(_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters.Count + "/" + 2);
                    }
                    else
                    {
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters = new List<GameObject>(1)
                        {
                            smallMonster
                        };
                        //Debug.Log(_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().monsters.Count + "/" + 1);
                    }

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
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().GenerateVillage();
                    }
                    // Update village to town
                    else if (   _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.VILLAGE &&
                                Random.Range(0, 100) <= villageUpgradeChance * multiplier)
                    {
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().GenerateTown();
                    }
                    // Update to ruins
                    else if (   (_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.VILLAGE &&
                                Random.Range(0, 100) <= villageRuinChance * multiplier) ||
                                (_world[xIndex, yIndex].GetComponent<SegmentBehaviour>().tileType == SegmentBehaviour.TileCategory.TOWN &&
                                Random.Range(0, 100) <= townRuinChance * multiplier) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x / 3) && _timePeriod >= 1) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x / 3) * 2 && _timePeriod >= 2) ||
                                (xIndex == Mathf.RoundToInt(worldSize.x - 2) && _timePeriod == maxTimePeriod - 1))
                    {
                        _world[xIndex, yIndex].GetComponent<SegmentBehaviour>().GenerateRuins();
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
    /// Returns direction
    /// </summary>
    /// <returns></returns>
    internal Vector2 GetDirection()
    {
        return _direction;
    }
    
    internal SegmentBehaviour GetCheckpoint()
    {
        return _world[Mathf.RoundToInt(_checkpointSegment.x), Mathf.RoundToInt(_checkpointSegment.y)].GetComponent<SegmentBehaviour>();
    }

    internal Vector2 GetCheckpointPos()
    {
        return _checkpointSegment;
    }

    /// <summary>
    /// Moves tile selection
    /// </summary>
    /// <param name="direction"></param>
    public void MoveSelection(Vector2 direction)
    {
        _direction = -direction;

        _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].transform.localScale = _tileScale;

        // Move selection
        _selectedTile += direction;

        // Make selected tile visible
        _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.VISIBLE);

        // Preview surrounding tiles
        //if (_world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>()._northEntrance &&
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y) - 1].GetComponent<SegmentBehaviour>().displayType == SegmentBehaviour.DisplayCategory.INVISIBLE)
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y) - 1].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.PREVIEW);
        //}
        //if (_world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>()._southEntrance &&
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y) + 1].GetComponent<SegmentBehaviour>().displayType == SegmentBehaviour.DisplayCategory.INVISIBLE)
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y) + 1].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.PREVIEW);
        //}
        //if (_world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>()._eastEntrance &&
        //    _world[Mathf.RoundToInt(_selectedTile.x) + 1, Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().displayType == SegmentBehaviour.DisplayCategory.INVISIBLE)
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x) + 1, Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.PREVIEW);
        //}
        //if (_world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>()._westEntrance &&
        //    _world[Mathf.RoundToInt(_selectedTile.x) - 1, Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().displayType == SegmentBehaviour.DisplayCategory.INVISIBLE)
        //{
        //    _world[Mathf.RoundToInt(_selectedTile.x) - 1, Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.PREVIEW);
        //}
    }

    public void SetSelectedToCheckpoint()
    {
        _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].transform.localScale = _tileScale;

        // Move selection
        _selectedTile = _checkpointSegment;

        // Make selected tile visible
        _world[Mathf.RoundToInt(_selectedTile.x), Mathf.RoundToInt(_selectedTile.y)].GetComponent<SegmentBehaviour>().UpdateDisplayType(SegmentBehaviour.DisplayCategory.VISIBLE);
    }

    /// <summary>
    /// Set checkpoint tile
    /// </summary>
    /// <param name="tile"></param>
    public void SetCheckpoint(GameObject tile)
    {
        for (int x = 0; x < worldSize.x; ++x)
        {
            for (int y = 0; y < worldSize.y; ++y)
            {
                if (_world[x, y] &&
                    _world[x,y] == tile)
                {
                    _checkpointSegment = new Vector2(x, y);
                    break;
                }
            }
        }
    }

    public int GetTimePeriod()
    {
        return _timePeriod;
    }

    public int GetDeadline()
    {
        return maxTimePeriod;
    }

    public int GetYearsRemaining()
    {
        return (maxTimePeriod - _timePeriod) * 50;
    }
}
