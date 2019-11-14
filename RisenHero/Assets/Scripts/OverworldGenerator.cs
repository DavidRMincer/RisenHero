using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2      worldSize;
    public int          startZone;
    public char         emptyChar = '.',
                        pathChar = '-',
                        forestChar = 'f',
                        villageChar = 'v',
                        townChar = 't',
                        castleChar = 'c',
                        ruinsChar = 'r',
                        startChar = 's',
                        endChar = 'x';

    private char[,]    _world;
    private Vector2     _startPoint;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
        Debug.Log(_world);
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
