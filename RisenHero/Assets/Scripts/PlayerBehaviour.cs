using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<GameObject> partyMembers;

    internal bool           inputEnabled = false;

    // Update is called once per frame
    void Update()
    {
        //Input movement
        if (inputEnabled)
        {
            Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
    }

    public void SpawnCompanions(Vector2 centre)
    {
        // DO STUFF
    }
}
