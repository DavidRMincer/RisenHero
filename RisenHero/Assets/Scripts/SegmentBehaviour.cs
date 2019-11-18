using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public enum TileCategory
    {
        EMPTY, START, END, FOREST, RUINS, WALL, VILLAGE, TOWN, CASTLE
    };

    public TileCategory tileType;
    public Vector2      minSize,
                        maxSize;
    public char         emptyChar = '.',
                        pathChar = '-',
                        cliffChar = '/',
                        rubbleChar = 'R',
                        treeChar = 't',
                        bushChar = 'b',
                        rockChar = 'r',
                        waterChar = 'w',
                        billboardChar = 'B',
                        shopChar = 's',
                        innChar = 'i',
                        houseChar = 'h',
                        checkpointChar = '!',
                        exitChar = 'X';
    public GameObject   cliffPrefab,
                        treePrefab,
                        rockPrefab,
                        bushPrefab;
    public int          minTrees,
                        maxTrees,
                        minRocks,
                        maxRocks,
                        minBushes,
                        maxBushes,
                        pathRadius,
                        tileSize;

    private string      _name;
    private char[,]     _segment;
    private Vector2     _segSize;
    public bool         _northEntrance,
                        _southEntrance,
                        _eastEntrance,
                        _westEntrance;

    // Start is called before the first frame update
    void Start()
    {
        GenerateForest();
        DrawSegment();
        SegmentArrayDebug();
    }

    /// <summary>
    /// Procedurally generate segment
    /// </summary>
    public void GenerateSegment()
    {
        // Set segment size
        _segSize = new Vector2(Random.Range(minSize.x, maxSize.x), Random.Range(minSize.y, maxSize.y));
        _segment = new char[Mathf.RoundToInt(_segSize.x), Mathf.RoundToInt(_segSize.y)];

        // Fill segment with empty chars
        for (int xIndex = 0; xIndex < Mathf.RoundToInt(_segSize.x); ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(_segSize.y); ++yIndex)
            {
                if ((_northEntrance && xIndex == Mathf.RoundToInt(_segSize.x) / 2 && yIndex == 0) ||
                    (_southEntrance && xIndex == Mathf.RoundToInt(_segSize.x) / 2 && yIndex == Mathf.RoundToInt(_segSize.y) - 1) ||
                    (_eastEntrance && xIndex == Mathf.RoundToInt(_segSize.x) - 1 && yIndex == Mathf.RoundToInt(_segSize.y) / 2) ||
                    (_westEntrance && xIndex == 0 && yIndex == Mathf.RoundToInt(_segSize.y) / 2))
                {
                    _segment[xIndex, yIndex] = exitChar;
                }
                else if (xIndex == 0 ||
                    yIndex == 0 ||
                    xIndex == Mathf.RoundToInt(_segSize.x) - 1 ||
                    yIndex == Mathf.RoundToInt(_segSize.y) - 1)
                {                    
                    _segment[xIndex, yIndex] = cliffChar;
                }
                else
                {
                    _segment[xIndex, yIndex] = emptyChar;
                }
            }
        }
    }

    public void GenerateForest()
    {
        GenerateSegment();

        // Fill with trees
        for (int xIndex = 1; xIndex < Mathf.RoundToInt(_segSize.x) - 1; ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(_segSize.y) - 1; ++yIndex)
            {
                if (_segment[xIndex, yIndex] == emptyChar)
                {
                    _segment[xIndex, yIndex] = treeChar;
                }
            }
        }

        // Clear path
    }

    public void DrawSegment()
    {
        for (int xIndex = 0; xIndex < _segSize.x; ++xIndex)
        {
            for (int yIndex = 0; yIndex < _segSize.y; ++yIndex)
            {
                Debug.Log(xIndex + ", " + yIndex);
                if (_segment[xIndex, yIndex] == cliffChar)
                {
                    Instantiate(cliffPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                }
                else if (_segment[xIndex, yIndex] == treeChar)
                {
                    Instantiate(treePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                }
                else if (_segment[xIndex, yIndex] == rockChar)
                {
                    Instantiate(rockPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                }
                else if (_segment[xIndex, yIndex] == bushChar)
                {
                    Instantiate(bushPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                }
            }
        }
    }

    /// <summary>
    /// Prints _segment in the debug log
    /// </summary>
    private void SegmentArrayDebug()
    {
        for (int y = 0; y < Mathf.RoundToInt(_segSize.y); y++)
        {
            string row = " ";

            for (int x = 0; x < Mathf.RoundToInt(_segSize.x); x++)
            {
                row += _segment[x, y];
            }

            Debug.Log(row);
        }
    }
}
