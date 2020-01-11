using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneBehaviour : MonoBehaviour
{
    public Text         textBox,
                        subTextBox;
    public Image        blackoutImage;
    public List<string> lines,
                        subLines;
    public float        textDuration,
                        fadeSpeed,
                        blackoutSpeed;
    public Color        transparent;
    public string       nextScene;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayCutscene());
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

    private IEnumerator PlayCutscene()
    {
        StartCoroutine(FadeTo(transparent, blackoutSpeed));
        yield return new WaitForSeconds(blackoutSpeed);

        float counter = 0f;

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < lines.Count; ++i)
        {
            textBox.text = lines[i];

            if (subLines.Count > i)
            {
                subTextBox.text = subLines[i];
            }

            while (counter < fadeSpeed)
            {
                counter += Time.deltaTime;
                counter = (counter > fadeSpeed ? 1 : counter);

                textBox.color = Color.Lerp(transparent, Color.white, counter / fadeSpeed);
                if (subLines.Count > i)
                    subTextBox.color = Color.Lerp(transparent, Color.white, counter / fadeSpeed);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(textDuration);
            counter = 0f;

            while (counter < fadeSpeed)
            {
                counter += Time.deltaTime;
                counter = (counter > fadeSpeed ? 1 : counter);

                textBox.color = Color.Lerp(Color.white, transparent, counter / fadeSpeed);
                if (subLines.Count > i)
                    subTextBox.color = Color.Lerp(Color.white, transparent, counter / fadeSpeed);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(textDuration);
            counter = 0f;
        }
        
        FadeTo(Color.black, blackoutSpeed);
        yield return new WaitForSeconds(blackoutSpeed + 1f);

        SceneManager.LoadScene(nextScene);
    }
}
