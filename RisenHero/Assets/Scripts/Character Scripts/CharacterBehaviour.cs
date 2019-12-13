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

    internal Rigidbody2D            _rb;

    private int                     _currentHealth,
                                    _currentActionCooldown;

    // Start is called before the first frame update
    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].user = this;
        }
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

    public int GetHealth()
    {
        return _currentHealth;
    }

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

    public void ReplenishCooldown()
    {
        _currentActionCooldown = actionCooldown;
    }
}
