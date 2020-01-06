using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBehaviour : MonoBehaviour
{
    public Image movementInputImg;
    public float movementTutorialDuration;

    private bool _moved = false;
    
    private void LateUpdate()
    {
        if (!_moved)
        {
            _moved = (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0);
        }

        movementInputImg.gameObject.SetActive(Time.time >= movementTutorialDuration && !_moved);
    }
}
