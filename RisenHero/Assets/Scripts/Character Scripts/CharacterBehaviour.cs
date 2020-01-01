using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class CharacterBehaviour : MonoBehaviour
{
    public float                    walkSpeed;
    public int                      maxHealth,
                                    damage,
                                    actionCooldown;
    public List<ActionBehaviour>    actions;
    public Renderer                 rend;

    internal Rigidbody2D            _rb;

    private int                     _currentHealth;
    private Vector2                 _directionFacing = Vector2.right;
    
    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].user = this;
            actions[i].cooldown = actionCooldown;
        }
    }
    
    /// <summary>
    /// Move in direction at speed
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Move(Vector2 direction)
    {
        _rb.velocity = direction.normalized * walkSpeed * Time.deltaTime;

        if (direction != Vector2.zero)
        {
            _directionFacing = direction;
        }
    }

    /// <summary>
    /// Zeros velocity
    /// </summary>
    public void StopMoving()
    {
        _rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Returns current health
    /// </summary>
    /// <returns></returns>
    public int GetHealth()
    {
        return _currentHealth;
    }

    /// <summary>
    /// Add to health
    /// </summary>
    /// <param name="health"></param>
    public void AddHealth(int health)
    {
        _currentHealth += health;

        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
        else if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }
    }

    /// <summary>
    /// Sets cooldown to max
    /// </summary>
    public void ReplenishCooldown()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].ReplenishCooldown();
        }
    }

    /// <summary>
    /// Returns Vector2 of facing direction
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirectionFacing()
    {
        return _directionFacing;
    }

    /// <summary>
    /// Sets _directionFacing
    /// </summary>
    /// <param name="dir"></param>
    public void SetDirectionFacing(Vector2 dir)
    {
        _directionFacing = dir.normalized;
    }
}
