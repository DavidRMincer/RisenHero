using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{
    public enum ActionType
    {
        ATTACK, HEAVY_ATTACK, HEAL
    };

    public ActionType           type;

    internal CharacterBehaviour user;
    internal int                cooldown = 0;

    private int                 _currentCooldown = 0;

    public void Perform(CharacterBehaviour target)
    {
        if (target && user &&
            _currentCooldown >= cooldown)
        {
            switch (type)
            {
                case ActionType.ATTACK:
                    target.AddHealth(-user.damage);
                    break;
                case ActionType.HEAVY_ATTACK:
                    target.AddHealth(-(user.damage + (user.damage / 2)));
                    break;
                case ActionType.HEAL:
                    target.AddHealth(user.damage);
                    break;
                default:
                    break;
            }

            _currentCooldown = 0;
        }
    }

    public void UpdateCooldown()
    {
        ++_currentCooldown;
    }

    public void ReplenishCooldown()
    {
        _currentCooldown = cooldown;
    }
}
