using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<GameObject> partyMembers = new List<GameObject>();

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

    public void AddCompanion(GameObject newCompanion)
    {
        newCompanion.GetComponent<CompanionBehaviour>().leader = this;


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
}
