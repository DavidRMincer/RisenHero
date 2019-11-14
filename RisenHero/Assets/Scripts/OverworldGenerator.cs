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

    private void Branch(int segments, bool major)
    {
        //DO STUFF
    }
}
