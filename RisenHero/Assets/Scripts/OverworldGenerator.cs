using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2      worldSize;
    public const char   emptyChar = '.',
                        pathChar = '-',
                        forestChar = 'f',
                        villageChar = 'v',
                        townChar = 't',
                        castleChar = 'c',
                        ruinsChar = 'r',
                        startChar = 's',
                        endChar = 'x';
    public float        tileSize;
    public GameObject   emptyTile,
                        pathTile,
                        forestTile,
                        villageTile,
                        townTile,
                        castleTile,
                        ruinsTile,
                        startTile,
                        endTile;

    private char[,]    _world;
    private Vector2     _startPoint,
                        _endPoint;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
        DrawWorld();
    }

    /// <summary>
    /// Procedurally generate overworld
    /// </summary>
    public void GenerateWorld()
    {
        char[,] data = new char[Mathf.RoundToInt(worldSize.x),Mathf.RoundToInt(worldSize.y)];
        _world = data;

        // Create blank world
        for (int xIndex = 0; xIndex < worldSize.x; ++xIndex)
        {
            for (int yIndex = 0; yIndex < worldSize.y; ++yIndex)
            {
                _world[xIndex, yIndex] = emptyChar;
            }
        }

        // Set start point
        _startPoint.x = 0;
        _startPoint.y = Mathf.RoundToInt(worldSize.y / 2);
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y)] = startChar;

        // Add path below start point
        // Through forest to village
        _world[Mathf.RoundToInt(_startPoint.x) + 1, Mathf.RoundToInt(_startPoint.y)] = forestChar;
        _world[Mathf.RoundToInt(_startPoint.x) + 2, Mathf.RoundToInt(_startPoint.y)] = forestChar;
        _world[Mathf.RoundToInt(_startPoint.x) + 3, Mathf.RoundToInt(_startPoint.y)] = villageChar;

        // Generate branching path
        BranchPath(Mathf.RoundToInt(_startPoint.x) + 3, Mathf.RoundToInt(_startPoint.y), true);
    }

    /// <summary>
    /// Draw tiles into scene
    /// </summary>
    public void DrawWorld()
    {
        // Draw each tile
        for (int xIndex = 0; xIndex < worldSize.x; ++xIndex)
        {
            for (int yIndex = 0; yIndex < worldSize.y; ++yIndex)
            {
                switch (_world[xIndex, yIndex])
                {
                    case pathChar:
                        GameObject.Instantiate(pathTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case forestChar:
                        GameObject.Instantiate(forestTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case villageChar:
                        GameObject.Instantiate(villageTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case townChar:
                        GameObject.Instantiate(townTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case castleChar:
                        GameObject.Instantiate(castleTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case ruinsChar:
                        GameObject.Instantiate(ruinsTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case startChar:
                        GameObject.Instantiate(startTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    case endChar:
                        GameObject.Instantiate(endTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;

                    default:
                        GameObject.Instantiate(emptyTile, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                }
            }
        }
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
            yIndex = y;

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

                _world[xIndex + xDir, yIndex + yDir] == emptyChar)
            {
                xIndex += xDir;
                yIndex += yDir;

                if (major &&
                    xIndex == worldSize.x - 1)
                {
                    _endPoint.x = xIndex;
                    _endPoint.y = yIndex;

                    _world[xIndex, yIndex] = endChar;

                    complete = true;
                }
                else
                {
                    _world[xIndex, yIndex] = forestChar;
                }
            }
        }
    }
}
