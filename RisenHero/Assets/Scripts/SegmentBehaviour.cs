using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public enum DisplayCategory
    {
        INVISIBLE, PREVIEW, VISIBLE
    };

    public enum TileCategory
    {
        EMPTY, START, END, FOREST, RUINS, WALL, VILLAGE, TOWN
    };

    public TileCategory         tileType;
    public Vector2              segSize;
    public GameObject           cliffPrefab,
                                treePrefab,
                                rockPrefab,
                                bushPrefab,
                                housePrefab,
                                shopPrefab,
                                innPrefab,
                                rubblePrefab,
                                wallPrefab,
                                checkpointPrefab,
                                threatPrefab,
                                exitPrefab;
    public int                  pathRadius,
                                tileSize,
                                minClearanceRadius,
                                maxClearanceRadius,
                                minHouses,
                                maxHouses,
                                minMonsters,
                                maxMonsters;
    public Sprite               blankSprite;

    internal DisplayCategory    displayType;
    internal bool               _northEntrance,
                                _southEntrance,
                                _eastEntrance,
                                _westEntrance;
    internal Vector2            checkpointTile = Vector2.zero;
    internal List<GameObject>   monsters = new List<GameObject>();

    private string              _name;
    private char[,]             _segment = new char[0, 0];
    private int                 _radius = 0;
    private Vector2             _centre;
    private Sprite              _tileSprite;
    private const char          _emptyChar = '.',
                                _pathChar = '-',
                                _cliffChar = '/',
                                _rubbleChar = 'R',
                                _treeChar = 't',
                                _bushChar = 'b',
                                _rockChar = 'r',
                                _shopChar = 's',
                                _innChar = 'i',
                                _houseChar = 'h',
                                _checkpointChar = '!',
                                _threatChar = 'x',
                                _wallChar = 'w',
                                _exitChar = 'X';
    private List<GameObject>    _listofObjects;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(monsters.Count);

        if (_listofObjects == null)
        {
            _listofObjects = new List<GameObject>(Mathf.RoundToInt(segSize.x * segSize.y) + maxMonsters);
        }

        _tileSprite = GetComponent<SpriteRenderer>().sprite;
        //GenerateSegment();
        //DrawSegment();
    }

    /// <summary>
    /// Generate segment based on tileType
    /// </summary>
    public void GenerateSegment()
    {
        if (_listofObjects == null)
        {
            _listofObjects = new List<GameObject>(Mathf.RoundToInt(segSize.x * segSize.y));
        }

        switch (tileType)
        {
            case TileCategory.EMPTY:
                break;
            case TileCategory.START:
                GenerateStart();
                break;
            case TileCategory.END:
                GenerateEnd();
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
            default:
                break;
        }

        SetCheckpointTile();
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
                    _segment[xIndex, yIndex] = _exitChar;
                }
                else if ((_northEntrance && xIndex >= Mathf.RoundToInt(_centre.x) - 1 && xIndex <= Mathf.RoundToInt(_centre.x) + 1 && yIndex == 0) ||
                    (_southEntrance && xIndex >= Mathf.RoundToInt(_centre.x) - 1 && xIndex <= Mathf.RoundToInt(_centre.x) + 1 && yIndex == Mathf.RoundToInt(segSize.y) - 1) ||
                    (_westEntrance && yIndex >= Mathf.RoundToInt(_centre.y) - 1 && yIndex <= Mathf.RoundToInt(_centre.y) + 1 && xIndex == 0) ||
                    (_eastEntrance && yIndex >= Mathf.RoundToInt(_centre.y) - 1 && yIndex <= Mathf.RoundToInt(_centre.y) + 1 && xIndex == Mathf.RoundToInt(segSize.x) - 1))
                {
                    _segment[xIndex, yIndex] = _emptyChar;
                }
                else if (xIndex == 0 ||
                    yIndex == 0 ||
                    xIndex == Mathf.RoundToInt(segSize.x) - 1 ||
                    yIndex == Mathf.RoundToInt(segSize.y) - 1)
                {                    
                    _segment[xIndex, yIndex] = _cliffChar;
                }
                else
                {
                    _segment[xIndex, yIndex] = _treeChar;
                }
            }
        }

        _centre = segSize / 2;
    }

    /// <summary>
    /// Generate starting segment
    /// </summary>
    public void GenerateStart()
    {
        // Generate forest
        SetupSegment();

        // Set centre of radius
        Vector2 radiusCentre = (_centre * Vector2.up) + (Vector2.right * maxClearanceRadius);

        // Clear starting radius
        ClearRadius(radiusCentre);

        // Add checkpoint
        _segment[Mathf.RoundToInt(radiusCentre.x), Mathf.RoundToInt(radiusCentre.y)] = _checkpointChar;
    }

    /// <summary>
    /// Generate end segment
    /// </summary>
    public void GenerateEnd()
    {
        SetupSegment();
        ClearRadius(_centre);

        for (int y = 1; y < segSize.y - 1; ++y)
        {
            for (int x = 1; x < segSize.x - 1; ++x)
            {
                // Place threat in centre
                if (x == Mathf.RoundToInt(_centre.x) &&
                    y == Mathf.RoundToInt(_centre.y))
                {
                    _segment[x, y] = _threatChar;
                }
                // Replace trees with cliffs
                else if (_segment[x, y] == _treeChar)
                {
                    _segment[x, y] = _cliffChar;
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

                // If more than 1 exit...
                // Randomly decide if another path is generated
                if (listofExits.Count > 1)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        GeneratePath(listofExits[i], new Vector2(Random.Range(segSize.x / 3, (segSize.x / 3) * 2), Random.Range(segSize.y / 3, (segSize.y / 3) * 2)));
                    }
                }
                // If only one exit...
                // Randomly generate at least 2 paths
                else
                {
                    int numofPaths = Random.Range(2, 5);

                    for (int k = 0; k <= numofPaths; ++k)
                    {
                        GeneratePath(listofExits[i], new Vector2(Random.Range(segSize.x / 3, (segSize.x / 3) * 2), Random.Range(segSize.y / 3, (segSize.y / 3) * 2)));
                    }
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
        ClearRadius(_centre);
        
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

            _segment[Mathf.RoundToInt(centre.x + (angle.x * distance)), Mathf.RoundToInt(centre.y + (angle.y * distance))] = _houseChar;
        }
    }

    /// <summary>
    /// Generate town segment
    /// </summary>
    public void GenerateTown()
    {
        SetupSegment();
        ClearRadius(_centre);
        
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
                char newChar = (Mathf.Sqrt((x * x) + (y * y)) <= distance) ? _shopChar : _houseChar;

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
                        (_segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] == _houseChar ||
                        _segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] == _shopChar))
                    {
                        _segment[xInn + (x * Mathf.RoundToInt(dirFromCentre.x)), yInn + (y * Mathf.RoundToInt(dirFromCentre.y))] = _innChar;
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
                if (_segment[x, y] == _houseChar ||
                    _segment[x, y] == _shopChar ||
                    _segment[x, y] == _innChar ||
                    _segment[x, y] == _wallChar)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        _segment[x, y] = _rubbleChar;
                    }
                    else
                    {
                        _segment[x, y] = _emptyChar;
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
        ClearRadius(_centre);

        // Generate wall
        for (int i = 0; i < Mathf.RoundToInt(segSize.y); i++)
        {
            _segment[Mathf.RoundToInt(_centre.x), i] = _wallChar;
        }

        // Add checkpoint
        _segment[Mathf.RoundToInt(_centre.x) - 2, Mathf.RoundToInt(_centre.y)] = _checkpointChar;
    }
    
    /// <summary>
    /// Clear random radius in trees
    /// </summary>
    private void ClearRadius(Vector2 radiusCentre)
    {
        _radius = Random.Range(minClearanceRadius, maxClearanceRadius);

        for (int x = 1; x < Mathf.RoundToInt(segSize.x) - 1; ++x)
        {
            for (int y = 1; y < Mathf.RoundToInt(segSize.y) - 1; ++y)
            {
                int xDist = x - Mathf.RoundToInt(radiusCentre.x),
                    yDist = y - Mathf.RoundToInt(radiusCentre.y),
                    distance = Mathf.RoundToInt(Mathf.Sqrt((xDist * xDist) + (yDist * yDist)));

                if (distance <= _radius)
                {
                    _segment[x, y] = _emptyChar;
                }
            }
        }

        // Paths from entrances
        if (_northEntrance)
        {
            PathPoints(new Vector2(segSize.x / 2, 0), radiusCentre);
        }
        if (_southEntrance)
        {
            PathPoints(new Vector2(segSize.x / 2, segSize.y - 1), radiusCentre);
        }
        if (_eastEntrance)
        {
            PathPoints(new Vector2(segSize.x - 1, segSize.y / 2), radiusCentre);
        }
        if (_westEntrance)
        {
            PathPoints(new Vector2(0, segSize.y / 2), radiusCentre);
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

            if (_segment[x, y] != _cliffChar &&
                _segment[x, y] != _exitChar)
            {
                _segment[x, y] = _pathChar;
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
                if (_segment[x, y] == _pathChar)
                {
                    for (int i = x - pathRadius; i <= x + pathRadius; ++i)
                    {
                        for (int j = y - pathRadius; j <= y + pathRadius; ++j)
                        {
                            if (x > 0 &&
                                x < segSize.x &&
                                y > 0 &&
                                y < segSize.y &&
                                _segment[i, j] == _treeChar)
                            {
                                _segment[i, j] = _emptyChar;
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
        // Instantiate tiles
        for (int xIndex = 0; xIndex < Mathf.RoundToInt(segSize.x); ++xIndex)
        {
            for (int yIndex = 0; yIndex < Mathf.RoundToInt(segSize.y); ++yIndex)
            {
                switch (_segment[xIndex, yIndex])
                {
                    case _cliffChar:
                        _listofObjects.Add(Instantiate(cliffPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _treeChar:
                        _listofObjects.Add(Instantiate(treePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _rockChar:
                        _listofObjects.Add(Instantiate(rockPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _bushChar:
                        _listofObjects.Add(Instantiate(bushPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _houseChar:
                        _listofObjects.Add(Instantiate(housePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _shopChar:
                        _listofObjects.Add(Instantiate(shopPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _innChar:
                        _listofObjects.Add(Instantiate(innPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _rubbleChar:
                        _listofObjects.Add(Instantiate(rubblePrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _wallChar:
                        _listofObjects.Add(Instantiate(wallPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _checkpointChar:
                        _listofObjects.Add(Instantiate(checkpointPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _threatChar:
                        _listofObjects.Add(Instantiate(threatPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.identity));
                        break;
                    case _exitChar:
                        Vector2 direction = _centre - new Vector2(xIndex, yIndex);
                        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                        _listofObjects.Add(Instantiate(exitPrefab, new Vector2(xIndex, -yIndex) * tileSize, Quaternion.AngleAxis(angle, Vector3.forward)));
                        break;
                    default:
                        break;
                }
            }
        }

        for (int i = 0; i < Random.Range(minMonsters, maxMonsters); ++i)
        {
            int xPos = Random.Range(1, Mathf.RoundToInt(segSize.x - 1)),
                yPos = Random.Range(1, Mathf.RoundToInt(segSize.y - 1));
            bool completed = false;

            for (int x = xPos; x < Mathf.RoundToInt(segSize.x) && x > 0; ++x)
            {
                for (int y = yPos; y < Mathf.RoundToInt(segSize.y) && y > 0; ++y)
                {
                    if (!completed &&
                        _segment[x, y] ==_pathChar)
                    {
                        int mIndex = Random.Range(0, monsters.Count - 1);

                        Debug.Log((mIndex + 1) + " / " + monsters.Count);
                        _listofObjects.Add(Instantiate(monsters[mIndex], new Vector2(x, -y) * tileSize, Quaternion.identity));

                        completed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns centre point
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCentre()
    {
        return _centre * (Vector2.right - Vector2.up) * tileSize;
    }

    /// <summary>
    /// Set visibility
    /// </summary>
    /// <param name="newDisplay"></param>
    public void UpdateDisplayType(DisplayCategory newDisplay)
    {
        displayType = newDisplay;

        //switch (displayType)
        //{
        //    case DisplayCategory.INVISIBLE:
        //        GetComponent<SpriteRenderer>().enabled = false;
        //        break;
        //    case DisplayCategory.PREVIEW:
        //        GetComponent<SpriteRenderer>().sprite = blankSprite;
        //        GetComponent<SpriteRenderer>().enabled = true;
        //        break;
        //    case DisplayCategory.VISIBLE:
        //        GetComponent<SpriteRenderer>().sprite = _tileSprite;
        //        GetComponent<SpriteRenderer>().enabled = true;
        //        break;
        //    default:
        //        break;
        //}

        //Debug.Log(displayType);
    }

    /// <summary>
    /// Destroy all objects in list of objects
    /// </summary>
    public void UnloadSegment()
    {
        for (int i = 0; i < _listofObjects.Count; ++i)
        {
            if (_listofObjects[i])
            {
                Destroy(_listofObjects[i]);
            }
        }
    }

    public Vector2 GetCheckpointTile()
    {
        return checkpointTile;
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

    /// <summary>
    /// Set checkpointTile as position of checkpoint
    /// </summary>
    private void SetCheckpointTile()
    {
        for (int x = 0; x < Mathf.RoundToInt(segSize.x); ++x)
        {
            for (int y = 0; y < Mathf.RoundToInt(segSize.y); ++y)
            {
                if (_segment[x, y] == _checkpointChar)
                {
                    checkpointTile = new Vector2(x, y);
                    break;
                }
            }
        }
    }
}
