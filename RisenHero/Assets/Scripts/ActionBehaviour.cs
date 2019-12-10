using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{
    public enum ActionType
    {
        ATTACK, HEAL
    };

    public ActionType           type;

    internal CharacterBehaviour user;

    public void SetUser(CharacterBehaviour c)
    {
        user = c;
    }

    public void Perform(CharacterBehaviour target)
    {
        switch (type)
        {
            case ActionType.ATTACK:
                target.AddHealth(-user.damage);
                break;
            case ActionType.HEAL:
                target.AddHealth(user.damage);
                break;
            default:
                break;
        }
    }
}
