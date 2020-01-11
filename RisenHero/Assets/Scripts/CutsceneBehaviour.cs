using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneBehaviour : MonoBehaviour
{
    public Text         textBox;
    public List<string> lines;
    public float        textDuration,
                        fadeSpeed;
    public Color        transparent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        float counter = 0f;

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < lines.Count; ++i)
        {
            textBox.text = lines[i];

            while (counter < fadeSpeed)
            {
                counter += Time.deltaTime;
                counter = (counter > fadeSpeed ? 1 : counter);

                textBox.color = Color.Lerp(transparent, Color.white, counter / fadeSpeed);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(textDuration);
            counter = 0f;

            while (counter < fadeSpeed)
            {
                counter += Time.deltaTime;
                counter = (counter > fadeSpeed ? 1 : counter);

                textBox.color = Color.Lerp(Color.white, transparent, counter / fadeSpeed);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(textDuration);
            counter = 0f;
        }

        
    }
}
