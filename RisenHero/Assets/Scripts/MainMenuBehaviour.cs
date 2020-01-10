using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    public Image        blackOutImage,
                        title;
    public List<Button> buttons;
    public Color        black,
                        white,
                        transparent;
    public float        fadeDuration;

    void Start()
    {
        title.color = transparent;

        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].gameObject.SetActive(false);
        }

        StartCoroutine(OpenMenu());
    }

    public IEnumerator FadeTo(Image img, Color newColour)
    {
        Color oldColour = img.color;
        float counter = 0f;

        do
        {
            Debug.Log(counter);
            counter += Time.deltaTime;
            counter = (counter > fadeDuration ? fadeDuration : counter);

            img.color = Color.Lerp(oldColour, newColour, counter / fadeDuration);

            yield return new WaitForSeconds(Time.deltaTime);

        } while (counter < fadeDuration);
    }

    public IEnumerator OpenMenu()
    {
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeTo(blackOutImage, transparent));
        yield return new WaitForSeconds(fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        StartCoroutine(FadeTo(title, white));
        yield return new WaitForSeconds(fadeDuration);
        
        yield return new WaitForSeconds(fadeDuration);

        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].gameObject.SetActive(true);
        }
    }
}
