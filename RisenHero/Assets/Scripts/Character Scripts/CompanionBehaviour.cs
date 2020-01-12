using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : CharacterBehaviour
{
    public enum CompanionType
    {
        WARRIOR, MAGE, CHEF, BACKPACKER
    };

    public CompanionType        type;
    public float                minDistance,
                                maxDistance,
                                teleportDistance;

    private bool                _inRange;
    private PlayerBehaviour     _leader;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_leader)
        {
            if (!InRangeofLeader(teleportDistance))
            {
                transform.position = new Vector2(_leader.transform.position.x, _leader.transform.position.y) - _leader.GetDirectionFacing();
            }
            else if (!InRangeofLeader(_inRange ? maxDistance : minDistance))
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
    }

    private void LateUpdate()
    {
        if (_leader &&
            _rb.velocity == Vector2.zero)
        {
            rend.flipX = _leader.transform.position.x < transform.position.x;
        }
    }

    /// <summary>
    /// Returns true if in range.
    /// Range changes depending if already in range of leader.
    /// </summary>
    /// <returns></returns>
    private bool InRangeofLeader(float range)
    {
        float   xDist = _leader._rb.transform.position.x - _rb.transform.position.x,
                yDist = _leader._rb.transform.position.y - _rb.transform.position.y;

        float   distance = Mathf.Sqrt((xDist * xDist) + (yDist * yDist));

        return distance <= range;
    }

    /// <summary>
    /// Moves in direction of leader.
    /// Currently does not use pathfinding.
    /// </summary>
    private void Follow()
    {
        Vector2 direction = (_leader._rb.transform.position - _rb.transform.position).normalized;

        Move(direction);
    }

    /// <summary>
    /// Set leader
    /// </summary>
    /// <param name="leader"></param>
    public void SetLeader(PlayerBehaviour leader)
    {
        _leader = leader;
    }
}
