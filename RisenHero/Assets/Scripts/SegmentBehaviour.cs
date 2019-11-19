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
    public Vector2      segSize;
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
        _segment = new char[Mathf.RoundToInt(segSize.x), Mathf.RoundToInt(segSize.y)];

        // Fill segment with empty chars
        for (int xIndex = 0; xIndex < Mathf.RoundToInt(segSize.x); ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(segSize.y); ++yIndex)
            {
                if ((_northEntrance && xIndex == Mathf.RoundToInt(segSize.x) / 2 && yIndex == 0) ||
                    (_southEntrance && xIndex == Mathf.RoundToInt(segSize.x) / 2 && yIndex == Mathf.RoundToInt(segSize.y) - 1) ||
                    (_eastEntrance && xIndex == Mathf.RoundToInt(segSize.x) - 1 && yIndex == Mathf.RoundToInt(segSize.y) / 2) ||
                    (_westEntrance && xIndex == 0 && yIndex == Mathf.RoundToInt(segSize.y) / 2))
                {
                    _segment[xIndex, yIndex] = exitChar;
                }
                else if (xIndex == 0 ||
                    yIndex == 0 ||
                    xIndex == Mathf.RoundToInt(segSize.x) - 1 ||
                    yIndex == Mathf.RoundToInt(segSize.y) - 1)
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
        for (int xIndex = 1; xIndex < Mathf.RoundToInt(segSize.x) - 1; ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(segSize.y) - 1; ++yIndex)
            {
                if (_segment[xIndex, yIndex] == emptyChar)
                {
                    _segment[xIndex, yIndex] = treeChar;
                }
            }
        }

        // Clear path
        List<Vector2> listofExits = new List<Vector2>();

        if (_northEntrance)
        {
            listofExits.Add(new Vector2(Mathf.RoundToInt(segSize.x) / 2, 0));
        }
        if (_southEntrance)
        {
            listofExits.Add(new Vector2(Mathf.RoundToInt(segSize.x) / 2, Mathf.RoundToInt(segSize.y) - 1));
        }
        if (_eastEntrance)
        {
            listofExits.Add(new Vector2(Mathf.RoundToInt(segSize.y) - 1, Mathf.RoundToInt(segSize.x) / 2));
        }
        if (_westEntrance)
        {
            listofExits.Add(new Vector2(0, Mathf.RoundToInt(segSize.x) / 2));
        }

        for (int i = 0; i < listofExits.Count; i++)
        {
            for (int j = 0; j < listofExits.Count; j++)
            {
                if (i != j)
                {
                    GeneratePath(listofExits[i], listofExits[j], 1);
                }
            }
        }

    }

    /// <summary>
    /// Clears path through trees
    /// NEEDS FIXING
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="noiseScale"></param>
    private void GeneratePath(Vector2 startPoint, Vector2 endPoint, int noiseScale)
    {
        int x = Mathf.RoundToInt(startPoint.x),
            y = Mathf.RoundToInt(startPoint.y);

        while (x != Mathf.RoundToInt(endPoint.x) &&
                y != Mathf.RoundToInt(endPoint.y))
        {
            // Move towards endPoint
            if (x < endPoint.x)
            {
                ++x;
            }
            else if (x > endPoint.x)
            {
                --x;
            }

            if (y < endPoint.y)
            {
                ++y;
            }
            else if (y > endPoint.y)
            {
                --y;
            }

            int xCoord = Mathf.RoundToInt(x / (segSize.x - 3) * noiseScale),
                yCoord = Mathf.RoundToInt(y / (segSize.y - 3) * noiseScale);

            // Add path
            if (_segment[xCoord, yCoord] != cliffChar)
            {
                _segment[xCoord, yCoord] = pathChar;
            }
        }
    }

    public void DrawSegment()
    {
        for (int xIndex = 0; xIndex < Mathf.RoundToInt(segSize.x); ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(segSize.y); ++yIndex)
            {
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
        for (int y = 0; y < Mathf.RoundToInt(segSize.y); y++)
        {
            string row = " ";

            for (int x = 0; x < Mathf.RoundToInt(segSize.x); x++)
            {
                row += _segment[x, y];
            }

            Debug.Log(row);
        }
    }
}
