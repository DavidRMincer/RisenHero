using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class CharacterBehaviour : MonoBehaviour
{
    public float walkSpeed;

    internal Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Move in direction at speed
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Move(Vector2 direction)
    {
        _rb.velocity = direction * walkSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Zeros velocity
    /// </summary>
    public void StopMoving()
    {
        _rb.velocity = Vector2.zero;
    }
}
