using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<GameObject> partyMembers = new List<GameObject>();
    public CombatBehaviour  combatManager;
    public ParticleSystem   healParticles;

    internal bool           inputEnabled = true;
    
    void FixedUpdate()
    {
        // Input movement
        if (inputEnabled)
        {
            Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
        // Stop movement
        else if (_rb.velocity != Vector2.zero)
        {
            _rb.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Add companion to list of partyMembers
    /// </summary>
    /// <param name="companion"></param>
    public void AddCompanion(ref GameObject companion)
    {
        //Debug.Log(companion.transform.position);
        //GameObject newCompanion = Instantiate(companion, new Vector2(companion.transform.position.x, companion.transform.position.y), Quaternion.identity);
        companion.GetComponent<CompanionBehaviour>().SetLeader(this);
        
        partyMembers.Add(companion);
        //Debug.Log(companion.transform.position);
    }

    /// <summary>
    /// Despawn companions when segment closes
    /// </summary>
    public void DespawnCompanions()
    {
        for (int i = 0; i < partyMembers.Count; ++i)
        {
            partyMembers[i].SetActive(false);
        }
    }

    /// <summary>
    /// Spawn companions when segment loads
    /// </summary>
    /// <param name="direction"></param>
    public void SpawnCompanions(Vector2 direction)
    {
        for (int i = 0; i < partyMembers.Count; ++i)
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + direction;

            partyMembers[i].SetActive(true);
            partyMembers[i].transform.position = newPos;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            combatManager.Setup(this, collision.GetComponent<MonsterBehaviour>());
        }
    }
}
