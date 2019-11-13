using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    public Vector2      worldSize;
    public char         emptyChar = '.',
                        forestChar = 'f',
                        villageChar = 'v',
                        townChar = 't',
                        castleChar = 'c',
                        ruinsChar = 'r',
                        startChar = 's',
                        endChar = 'x';

    private char[][]    _world;
    private Vector2     _startPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
