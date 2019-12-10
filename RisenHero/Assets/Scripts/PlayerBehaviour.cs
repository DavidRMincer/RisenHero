using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<GameObject> partyMembers = new List<GameObject>();

    internal bool           inputEnabled = false;

    private Vector2         _directionFacing = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        //Input movement
        if (inputEnabled)
        {
            _directionFacing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Move(_directionFacing);
        }
    }

    public void AddCompanion(GameObject newCompanion)
    {
        newCompanion.GetComponent<CompanionBehaviour>().SetLeader(this);
        
        partyMembers.Add(newCompanion);
    }

    public void DespawnCompanions()
    {
        for (int i = 0; i < partyMembers.Count; ++i)
        {
            partyMembers[i].SetActive(false);
        }
    }

    public void SpawnCompanions(Vector2 direction)
    {
        for (int i = 0; i < partyMembers.Count; ++i)
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + direction;

            partyMembers[i].SetActive(true);
            partyMembers[i].transform.position = newPos;
        }
    }

    public Vector2 GetDirectionFacing()
    {
        return _directionFacing;
    }
}
