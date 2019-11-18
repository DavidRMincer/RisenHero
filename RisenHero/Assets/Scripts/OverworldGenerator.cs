using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2              worldSize;
    public float                tileSize;
    public GameObject           emptyTile,
                                wallTile,
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
                                maxGap;
    
    GameObject[,]               _world;
    private Vector2             _startPoint,
                                _endPoint;
    public List<GameObject>     _segmentstoSpawn = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Fill segments list
        for (int i = numofCastles; i > 0; --i)
        {
            Debug.Log(true);
            _segmentstoSpawn.Add(castleTile);
        }
        for (int i = numofRuins; i > 0; --i)
        {
            Debug.Log(true);
            _segmentstoSpawn.Add(ruinsTile);
        }
        for (int i = numofTowns; i > 0; --i)
        {
            Debug.Log(true);
            _segmentstoSpawn.Add(townTile);
        }
        for (int i = numofVillages; i > 0; --i)
        {
            Debug.Log(true);
            _segmentstoSpawn.Add(villageTile);
        }

        GenerateWorld();
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

            if ((yIndex + yDir >= 0 &&
                yIndex + yDir < worldSize.y) &&

                _world[xIndex + xDir, yIndex + yDir] == null)
            {
                xIndex += xDir;
                yIndex += yDir;

                // Reached end
                if (major &&
                    xIndex == worldSize.x - 1)
                {
                    _endPoint.x = xIndex;
                    _endPoint.y = yIndex;

                    _world[xIndex, yIndex] = GameObject.Instantiate(endTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);

                    complete = true;
                }
                // Generate segment
                else
                {
                    // Add final wall
                    if (xIndex == Mathf.RoundToInt(worldSize.x - 2))
                    {
                        AddWall(xIndex);

                        ++xIndex;
                    }
                    // Add wall
                    else if (xIndex == Mathf.RoundToInt(worldSize.x / 3) ||
                        xIndex == Mathf.RoundToInt(worldSize.x / 3) * 2)
                    {
                        AddWall(xIndex);

                        ++xIndex;
                        _world[xIndex, yIndex] = GameObject.Instantiate(forestTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);
                    }
                    // Spawn random segment
                    else if (gapCounter == 0)
                    {
                        _world[xIndex, yIndex] = GameObject.Instantiate(_segmentstoSpawn[Random.Range(0, _segmentstoSpawn.Count - 1)], new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);;

                        gapCounter = Random.Range(minGap, maxGap);
                    }
                    // Spawn end of world
                    else
                    {
                        _world[xIndex, yIndex] = GameObject.Instantiate(forestTile, new Vector2(xIndex, yIndex) * tileSize, Quaternion.identity);

                        --gapCounter;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Add vertical wall across world's y
    /// </summary>
    /// <param name="x"></param>
    void AddWall(int x)
    {
        for (int i = 0; i < worldSize.y; ++i)
        {
            _world[x, i] = GameObject.Instantiate(wallTile, new Vector2(x, i) * tileSize, Quaternion.identity);
        }
    }

    void AgeWorld(int multiplier)
    {
        for (int xIndex = 0; xIndex < worldSize.x; ++xIndex)
        {
            for (int yIndex = 0; yIndex < worldSize.y; ++yIndex)
            {
                //DO STUFF
            }
        }
    }
}
