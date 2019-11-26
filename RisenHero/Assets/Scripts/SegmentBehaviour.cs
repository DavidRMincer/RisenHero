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
                        billboardChar = 'B',
                        shopChar = 's',
                        innChar = 'i',
                        houseChar = 'h',
                        checkpointChar = '!',
                        exitChar = 'X';
    public GameObject   cliffPrefab,
                        treePrefab,
                        rockPrefab,
                        bushPrefab,
                        housePrefab;
    public int          pathRadius,
                        tileSize,
                        minVillageRadius,
                        maxVillageRadius,
                        minHouses,
                        maxHouses;

    private string      _name;
    private char[,]     _segment;
    private int         _radius = 0;
    public bool         _northEntrance,
                        _southEntrance,
                        _eastEntrance,
                        _westEntrance;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSegment();
        DrawSegment();
        SegmentArrayDebug();
    }

    public void GenerateSegment()
    {
        switch (tileType)
        {
            case TileCategory.EMPTY:
                break;
            case TileCategory.START:
                break;
            case TileCategory.END:
                break;
            case TileCategory.FOREST:
                GenerateForest();
                break;
            case TileCategory.RUINS:
                break;
            case TileCategory.WALL:
                break;
            case TileCategory.VILLAGE:
                GenerateVillage();
                break;
            case TileCategory.TOWN:
                GenerateTown();
                break;
            case TileCategory.CASTLE:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Procedurally generate segment
    /// </summary>
    private void SetupSegment()
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
                    _segment[xIndex, yIndex] = treeChar;
                }
            }
        }
    }

    /// <summary>
    /// Generate forest segment
    /// </summary>
    public void GenerateForest()
    {
        SetupSegment();

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
                    GeneratePath(listofExits[i], listofExits[j]);
                }

                if (Random.Range(0, 2) == 0)
                {
                    GeneratePath(listofExits[i], new Vector2(Random.Range(segSize.x / 3, (segSize.x / 3) * 2), Random.Range(segSize.y / 3, (segSize.y / 3) * 2)));
                }
            }
        }

        PadPaths();
    }
    
    /// <summary>
    /// Generate village segment
    /// </summary>
    public void GenerateVillage()
    {
        SetupSegment();

        // Clear radius around centre
        ClearRadius();
        
        // Random spawn houses
        Vector2 centre = segSize / 2;
        int numHouses = Random.Range(minHouses, maxHouses);
        float degrees = 360 / numHouses;

        for (float i = 0; i < 360; i += degrees)
        {
            Vector2 angle = Vector2.up;
            float   sin = Mathf.Sin(i * Mathf.Deg2Rad),
                    cos = Mathf.Cos(i * Mathf.Deg2Rad);
            angle.x = (angle.x * cos) - (angle.y * sin);
            angle.y = (angle.x * sin) + (angle.y * cos);

            int distance = Random.Range(_radius / 3, _radius / 2);

            _segment[Mathf.RoundToInt(centre.x + (angle.x * distance)), Mathf.RoundToInt(centre.y + (angle.y * distance))] = houseChar;
        }
    }

    public void GenerateTown()
    {
        SetupSegment();
        ClearRadius();


    }

    /// <summary>
    /// Clear random radius in trees
    /// </summary>
    private void ClearRadius()
    {
        _radius = Random.Range(minVillageRadius, maxVillageRadius);

        for (int x = 1; x < Mathf.RoundToInt(segSize.x) - 2; ++x)
        {
            for (int y = 1; y < Mathf.RoundToInt(segSize.y) - 2; ++y)
            {
                int xDist = x - (Mathf.RoundToInt(segSize.x) / 2),
                    yDist = y - (Mathf.RoundToInt(segSize.y) / 2),
                    distance = Mathf.RoundToInt(Mathf.Sqrt((xDist * xDist) + (yDist * yDist)));

                if (distance <= _radius)
                {
                    _segment[x, y] = emptyChar;
                }
            }
        }

        // Paths from entrances
        if (_northEntrance)
        {
            PathPoints(new Vector2(segSize.x / 2, 0), new Vector2(segSize.x / 2, segSize.y / 2));
        }
        if (_southEntrance)
        {
            PathPoints(new Vector2(segSize.x / 2, segSize.y - 1), segSize / 2);
        }
        if (_eastEntrance)
        {
            PathPoints(new Vector2(segSize.x - 1, segSize.y / 2), segSize / 2);
        }
        if (_westEntrance)
        {
            PathPoints(new Vector2(0, segSize.y / 2), segSize / 2);
        }

        PadPaths();
    }

    /// <summary>
    /// Clears path through trees
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="noiseScale"></param>
    private void GeneratePath(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 currentPos;
        int numofPoints = Random.Range(1, 3);

        currentPos.x = Mathf.RoundToInt(startPoint.x);
        currentPos.y = Mathf.RoundToInt(startPoint.y);

        while (numofPoints >= 0)
        {
            Vector2 randPoint = new Vector2(Random.Range(1, segSize.x - 2), Random.Range(1, segSize.y - 2));

            currentPos = PathPoints(currentPos, randPoint);
            
            --numofPoints;
        }

        PathPoints(currentPos, endPoint);
    }
    
    /// <summary>
    /// Connect 2 points with a path
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Vector2 PathPoints(Vector2 start, Vector2 end)
    {
        int x = Mathf.RoundToInt(start.x),
            y = Mathf.RoundToInt(start.y);

        while ( x != Mathf.RoundToInt(end.x) ||
                y != Mathf.RoundToInt(end.y))
        {
            if (x < Mathf.RoundToInt(end.x))
            {
                ++x;
            }
            else if (x > Mathf.RoundToInt(end.x))
            {
                --x;
            }

            if (y < Mathf.RoundToInt(end.y))
            {
                ++y;
            }
            else if (y > Mathf.RoundToInt(end.y))
            {
                --y;
            }

            if (_segment[x, y] != cliffChar)
            {
                _segment[x, y] = pathChar;
            }
        }

        return new Vector2(x, y);
    }

    /// <summary>
    /// Clear forest in pathRadius around paths
    /// </summary>
    private void PadPaths()
    {
        for (int x = 1; x < segSize.x - 1; ++x)
        {
            for (int y = 1; y < segSize.y - 1; ++y)
            {
                if (_segment[x, y] == pathChar)
                {
                    for (int i = x - pathRadius; i <= x + pathRadius; ++i)
                    {
                        for (int j = y - pathRadius; j <= y + pathRadius; ++j)
                        {
                            if (x > 0 &&
                                x < segSize.x &&
                                y > 0 &&
                                y < segSize.y &&
                                _segment[i, j] == treeChar)
                            {
                                _segment[i, j] = emptyChar;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Instantiate objects in segment
    /// </summary>
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
                else if (_segment[xIndex, yIndex] == houseChar)
                {
                    Instantiate(housePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
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
