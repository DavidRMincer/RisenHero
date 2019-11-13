using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : CharacterBehaviour
{
    public CharacterBehaviour   leader;
    public float                minDistance,
                                maxDistance;

    bool                        _inRange;

    // Update is called once per frame
    void Update()
    {
        if (!InRangeofLeader())
        {
            _inRange = false;
            Follow();
        }
        else
        {
            _inRange = true;
            StopMoving();
        }
    }

    /// <summary>
    /// Returns true if in range.
    /// Range changes depending if already in range of leader.
    /// </summary>
    /// <returns></returns>
    private bool InRangeofLeader()
    {
        float range = _inRange ? maxDistance : minDistance;

        float   xDist = leader._rb.transform.position.x - _rb.transform.position.x,
                yDist = leader._rb.transform.position.y - _rb.transform.position.y;

        float   distance = Mathf.Sqrt((xDist * xDist) + (yDist * yDist));

        return distance <= range;
    }

    /// <summary>
    /// Moves in direction of leader.
    /// Currently does not use pathfinding.
    /// </summary>
    private void Follow()
    {
        Vector2 direction = (leader._rb.transform.position - _rb.transform.position).normalized;

        Move(direction);
    }
}
