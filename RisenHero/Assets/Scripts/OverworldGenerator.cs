using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2      worldSize;
    public int          startZone;
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
    private Vector2     _startPoint;

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
        _startPoint.x = Random.Range(0, startZone);
        _startPoint.y = Random.Range(0, startZone);
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y)] = startChar;

        // Add path below start point
        // Through forest to village
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y) + 1] = pathChar;
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y) + 2] = forestChar;
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y) + 3] = pathChar;
        _world[Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y) + 4] = villageChar;

        // Calculate max number of path segments
        int pathNum = Mathf.RoundToInt(worldSize.x * worldSize.y) / 2;

        // Generate branching path
        BranchPath(pathNum, Mathf.RoundToInt(_startPoint.x), Mathf.RoundToInt(_startPoint.y + 4), true);
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
    private void BranchPath(int segments, int x, int y, bool major)
    {
        int xIndex = x,
            yIndex = y;

        do
        {
            int xDir = 0,
                yDir = 0;
            bool positive = (Random.Range(0, 2) == 0);

            // Set random direction
            if ((Random.Range(0, 2) == 0))
            {
                xDir = positive ? 1 : -1;
            }
            else
            {
                yDir = positive ? 1 : -1;
            }

            // If new space available...
            // Add path
            if ((xIndex + xDir >= 0 &&
                xIndex + xDir < worldSize.x &&
                yIndex + yDir >= 0 &&
                yIndex + yDir < worldSize.y &&
                _world[xIndex + xDir, yIndex + yDir] == emptyChar) &&

                (xIndex + (xDir * 2) >= 0 &&
                xIndex + (xDir * 2) < worldSize.x &&
                yIndex + (yDir * 2) >= 0 &&
                yIndex + (yDir * 2) < worldSize.y &&
                _world[xIndex + (xDir * 2), yIndex + (yDir * 2)] == emptyChar) &&

                (xIndex + xDir + yDir >= 0 &&
                xIndex + xDir + yDir < worldSize.x &&
                yIndex + yDir + xDir >= 0 &&
                yIndex + yDir + xDir < worldSize.y &&
                _world[xIndex + xDir + yDir, yIndex + yDir + xDir] == emptyChar) &&

                (xIndex + xDir - yDir >= 0 &&
                xIndex + xDir - yDir < worldSize.x &&
                yIndex + yDir - xDir >= 0 &&
                yIndex + yDir - xDir < worldSize.y &&
                _world[xIndex + xDir - yDir, yIndex + yDir - xDir] == emptyChar))
            {
                xIndex += xDir;
                yIndex += yDir;

                _world[xIndex, yIndex] = pathChar;
            }

            --segments;
        } while (segments > 0);

        if (major)
        {
            _world[xIndex, yIndex] = endChar;
        }
    }
}
