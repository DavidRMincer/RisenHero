using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBehaviour : MonoBehaviour
{
    public GameObject   healthBar;
    public Image        movementInputImg,
                        actionInputImg,
                        heartImage;
    public float        movementTutorialDuration,
                        heartGap;
    public Image[]      hearts;
    public Sprite       heartSprite,
                        shatteredHeartSprite;

    private bool _moved = false;

    private void Start()
    {
        actionInputImg.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!_moved)
        {
            _moved = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        }

        movementInputImg.gameObject.SetActive(Time.time >= movementTutorialDuration && !_moved);
    }

    public void SetupHearts(int health)
    {
        hearts = new Image[health];

        for (int i = 0; i < hearts.Length; ++i)
        {
            //Image newHeart = heartImage;
            //newHeart.transform.SetParent(this.transform, false);
            hearts[i] = Instantiate(heartImage, healthBar.GetComponent<RectTransform>().rect.position + (Vector2.right * heartGap * i), Quaternion.identity);
            hearts[i].transform.SetParent(healthBar.GetComponent<RectTransform>(), false);
        }
    }

    public void UpdateHealth(int health)
    {
        for (int i = 0; i < hearts.Length; ++i)
        {
            if (i < health)
            {
                hearts[i].sprite = heartSprite;
            }
            else
            {
                hearts[i].sprite = shatteredHeartSprite;
            }
        }
    }
}
