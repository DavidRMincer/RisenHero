using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBehaviour : MonoBehaviour
{
    public GameObject   healthBar,
                        pauseMenu;
    public Image        movementInputImg,
                        actionInputImg,
                        heartImage,
                        blackoutImage;
    public float        movementTutorialDuration,
                        heartGap,
                        scrollSpeed;
    public Image[]      hearts;
    public List<Button> menuButtons;
    public Sprite       heartSprite,
                        shatteredHeartSprite;
    public Color        blackout,
                        whiteout,
                        transparent;

    private float       _scrollCounter = 0f;
    private int         _menuIndex = 0;

    private bool _moved = false;

    private void Start()
    {
        actionInputImg.gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (pauseMenu.activeInHierarchy)
        //{
        //    if (_scrollCounter == 0f)
        //    {
        //        if (Input.GetAxis("Vertical") < 0)
        //        {
        //            --_menuIndex;
        //        }
        //        else if (Input.GetAxis("Vertical") > 0)
        //        {
        //            ++_menuIndex;
        //        }

        //        if (_menuIndex > menuButtons.Count)
        //        {
        //            _menuIndex = 0;
        //        }
        //        else if (_menuIndex < 0)
        //        {
        //            _menuIndex = menuButtons.Count;
        //        }
        //    }
        //    else
        //    {
        //        _scrollCounter -= Time.unscaledDeltaTime;
        //        _scrollCounter = (_scrollCounter < 0f ? 0 : _scrollCounter);
        //    }

        //    menuButtons[_menuIndex].Select();

        //    //for (int i = 0; i < menuButtons.Count; ++i)
        //    //{
        //    //    if (i == _menuIndex)
        //    //    {
        //    //        menuButtons[i].Select();
        //    //    }
        //    //}
        //}
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

    public IEnumerator FadeTo(Color newColour, float duration)
    {
        float counter = 0f;
        Color currentColour = blackoutImage.color;

        do
        {
            counter += Time.deltaTime;

            counter = (counter > duration ? duration : counter);

            blackoutImage.color = Color.Lerp(currentColour, newColour, counter / duration);

            yield return new WaitForSeconds(Time.deltaTime);

        } while (counter < duration);
    }

    public void SetHealthVisibility(bool visible)
    {
        for (int i = 0; i < hearts.Length; ++i)
        {
            hearts[i].gameObject.SetActive(visible);
        }
    }
}
