using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThreatBehaviour : MonoBehaviour
{
    public ParticleSystem               explosionParticles,
                                        smoke;
    public SpriteRenderer               auraSprite;
    public Color                        transparent;

    private GameManagerBehaviour        _gm;
    private UIManagerBehaviour          _uiM;
    private EnvironmentAssetBehaviour   _eAB;

    private void Start()
    {
        _gm = FindObjectOfType<GameManagerBehaviour>();
        _uiM = _gm.uiManager;
        _eAB = GetComponent<EnvironmentAssetBehaviour>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Press action_1 to set checkpoint
            if (_gm.player.GetComponent<PlayerBehaviour>().inputEnabled &&
                Input.GetButtonDown("Action_1"))
            {
                StartCoroutine(Defeat());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _uiM.actionInputImg.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _uiM.actionInputImg.gameObject.SetActive(false);
    }

    private IEnumerator Defeat()
    {
        _gm.player.GetComponent<PlayerBehaviour>().inputEnabled = false;
        _uiM.actionInputImg.gameObject.SetActive(false);
        _uiM.healthBar.SetActive(false);

        yield return new WaitForSeconds(1f);

        _eAB.rend.color = transparent;
        smoke.Stop();
        explosionParticles.Play();

        yield return new WaitForSeconds(2f);

        float counter = 0f;

        while (counter < 1f)
        {
            counter += Time.deltaTime;
            counter = (counter > 1 ? 1 : counter);

            auraSprite.color = Color.Lerp(Color.white, transparent, counter / 1);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(2f);

        _uiM.FadeTo(Color.black, 2f);
        yield return new WaitForSeconds(2f);

        LoadCredits();
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
