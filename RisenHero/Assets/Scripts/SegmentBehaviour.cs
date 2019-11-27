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
    public const char   emptyChar = '.',
                        pathChar = '-',
                        cliffChar = '/',
                        rubbleChar = 'R',
                        treeChar = 't',
                        bushChar = 'b',
                        rockChar = 'r',
                        shopChar = 's',
                        innChar = 'i',
                        houseChar = 'h',
                        checkpointChar = '!',
                        wallChar = 'w',
                        exitChar = 'X';
    public GameObject   cliffPrefab,
                        treePrefab,
                        rockPrefab,
                        bushPrefab,
                        housePrefab,
                        shopPrefab,
                        innPrefab,
                        rubblePrefab,
                        wallPrefab;
    public int          pathRadius,
                        tileSize,
                        minClearanceRadius,
                        maxClearanceRadius,
                        minHouses,
                        maxHouses;

    private string      _name;
    private char[,]     _segment = new char[0, 0];
    private int         _radius = 0;
    public bool         _northEntrance,
                        _southEntrance,
                        _eastEntrance,
                        _westEntrance;
    private Vector2     _centre;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSegment();
        DrawSegment();
        //SegmentArrayDebug();
    }

    /// <summary>
    /// Generate segment based on tileType
    /// </summary>
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
                GenerateRuins();
                break;
            case TileCategory.WALL:
                GenerateWall();
                break;
            case TileCategory.VILLAGE:
                GenerateVillage();
                break;
            case TileCategory.TOWN:
                GenerateTown();
                break;
            case TileCategory.CASTLE:
                GenerateCastle();
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

        _centre = segSize / 2;
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

    /// <summary>
    /// Generate town segment
    /// </summary>
    public void GenerateTown()
    {
        SetupSegment();
        ClearRadius();
        
        // Fit town inside cleared radius
        int houseWidth = Mathf.RoundToInt(housePrefab.GetComponent<SpriteRenderer>().size.x / tileSize) + (tileSize * 2),
            houseHeight = Mathf.RoundToInt(housePrefab.GetComponent<SpriteRenderer>().size.y / tileSize) + (tileSize * 3),
            townSize = LargestSquareinRadiusSide(),
            townWidth = (townSize / houseWidth),
            townHeight = (townSize / houseHeight);
        
        for (int x = 2; x < townWidth; x += houseWidth)
        {
            for (int y = 2; y < townHeight; y += houseHeight)
            {
                int xDist = Random.Range(2, townWidth / 3),
                    yDist = Random.Range(2, townHeight / 3),
                    distance = Mathf.RoundToInt(Mathf.Sqrt((xDist * xDist) + (yDist * yDist)));

                // Add houses and shops
                char newChar = (Mathf.Sqrt((x * x) + (y * y)) <= distance) ? shopChar : houseChar;

                _segment[Mathf.RoundToInt(_centre.x) + x, Mathf.RoundToInt(_centre.y) + y] = newChar;
                _segment[Mathf.RoundToInt(_centre.x) - x, Mathf.RoundToInt(_centre.y) + y] = newChar;
                _segment[Mathf.RoundToInt(_centre.x) + x, Mathf.RoundToInt(_centre.y) - y] = newChar;
                _segment[Mathf.RoundToInt(_centre.x) - x, Mathf.RoundToInt(_centre.y) - y] = newChar;
            }
        }

        // Add inn
        bool finished = false;

        do
        {
            int xInn = Mathf.RoundToInt(Random.Range(_centre.x - townWidth, _centre.x + townWidth)),
                yInn = Mathf.RoundToInt(Random.Range(_centre.y - townHeight, _centre.y + townHeight));
            Vector2 dirFromCentre = (_centre - new Vector2(xInn, yInn)).normalized;

            for (int x = 0; x < houseWidth * 5; ++x)
            {
                for (int y = 0; y < houseWidth * 5; ++y)
                {
                    if ((!finished) &&
                        (_segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] == houseChar ||
                        _segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] == shopChar))
                    {
                        _segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] = innChar;
                        finished = true;
                    }
                }
            }
        } while (!finished);
    }

    /// <summary>
    /// Generate ruins from village or town
    /// </summary>
    public void GenerateRuins()
    {
        // Create village or town to destroy
        if (_segment.GetLength(0) == 0)
        {
            if (Random.Range(0, 3) == 0)
            {
                GenerateVillage();
            }
            else
            {
                GenerateTown();
            }
        }

        for (int x = 1; x < Mathf.RoundToInt(segSize.x) - 1; ++x)
        {
            for (int y = 1; y < Mathf.RoundToInt(segSize.y) - 1; ++y)
            {
                if (_segment[x, y] == houseChar ||
                    _segment[x, y] == shopChar ||
                    _segment[x, y] == innChar ||
                    _segment[x, y] == wallChar)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        _segment[x, y] = rubbleChar;
                    }
                    else
                    {
                        _segment[x, y] = emptyChar;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generate wall segment
    /// </summary>
    public void GenerateWall()
    {
        // Block north and south exits
        _northEntrance = false;
        _southEntrance = false;

        SetupSegment();
        ClearRadius();

        // Generate wall
        for (int i = 0; i < Mathf.RoundToInt(segSize.y); i++)
        {
            _segment[Mathf.RoundToInt(_centre.x), i] = wallChar;
        }
    }

    public void GenerateCastle()
    {
        SetupSegment();
        ClearRadius();

        int castleSize = LargestSquareinRadiusSide() / 10 * Random.Range(8, 9),
            xStart = Mathf.RoundToInt(_centre.x) - (castleSize / 2),
            yStart = Mathf.RoundToInt(_centre.y) - (castleSize / 2);

        for (int x = 0; x < castleSize; ++x)
        {
            for (int y = 0; y < castleSize; ++y)
            {
                if (x == 0 ||
                    x == castleSize - 1 ||
                    y == 0 ||
                    y == castleSize - 1)
                {
                    _segment[xStart + x, yStart + y] = wallChar;
                }
            }
        }
    }

    /// <summary>
    /// Clear random radius in trees
    /// </summary>
    private void ClearRadius()
    {
        _radius = Random.Range(minClearanceRadius, maxClearanceRadius);

        for (int x = 1; x < Mathf.RoundToInt(segSize.x) - 1; ++x)
        {
            for (int y = 1; y < Mathf.RoundToInt(segSize.y) - 1; ++y)
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
    /// Returns one side of largest square that can fit in radius
    /// </summary>
    /// <returns></returns>
    private int LargestSquareinRadiusSide()
    {
        return Mathf.RoundToInt(_radius * Mathf.Sqrt(2));
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
                switch (_segment[xIndex, yIndex])
                {
                    case cliffChar:
                        Instantiate(cliffPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case treeChar:
                        Instantiate(treePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case rockChar:
                        Instantiate(rockPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case bushChar:
                        Instantiate(bushPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case houseChar:
                        Instantiate(housePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case shopChar:
                        Instantiate(shopPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case innChar:
                        Instantiate(innPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case rubbleChar:
                        Instantiate(rubblePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    case wallChar:
                        Instantiate(wallPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity);
                        break;
                    default:
                        break;
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
