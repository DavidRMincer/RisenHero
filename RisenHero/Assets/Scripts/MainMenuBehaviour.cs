using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuBehaviour : MonoBehaviour
{
    public Image        blackOutImage,
                        title;
    public List<Button> buttons;
    public Color        black,
                        white,
                        transparent;
    public float        fadeDuration;
    public string       nextScene;

    void Awake()
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

    public void LaunchBtn()
    {
        StartCoroutine(LaunchGame());
    }

    public void QuitBtn()
    {
        Application.Quit();
    }

    private IEnumerator LaunchGame()
    {
        FadeTo(blackOutImage, black);
        yield return new WaitForSeconds(fadeDuration);

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(nextScene);
    }
}
