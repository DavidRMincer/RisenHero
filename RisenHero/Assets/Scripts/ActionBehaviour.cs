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
        Debug.Log(type);
        switch (type)
        {
            case ActionType.ATTACK:
                target.AddHealth(-user.damage);
                Debug.Log(target.GetHealth());
                break;
            case ActionType.HEAL:
                target.AddHealth(user.damage);
                Debug.Log(target.GetHealth());
                break;
            default:
                break;
        }
    }
}
