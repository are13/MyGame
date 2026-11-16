using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager manager;

    [Header("Transition")]
    public Image fading;
    public float transitionTime = 0.5f;


    void Awake()
    {
        
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(manager != this)
        {
            
            Destroy(gameObject);
        }

        var temp = fading.color;
        temp.a = 0;
        fading.color = temp;
    }

    public void transformScene(String name)
    {
        
        StartCoroutine(FadeAndLoad(name));
    }

    private IEnumerator FadeAndLoad(String name)
    {
        
        yield return StartCoroutine(Fade(0f,1f));

        AsyncOperation loading = SceneManager.LoadSceneAsync(name);

        while (!loading.isDone)
        {
            
            yield return null;
        }

        yield return null;

        yield return StartCoroutine(Fade(1f,0f));
    }

    private IEnumerator Fade(float from,float to)
    {
        
        float timer = 0f;
        var temp = fading.color;
        while(timer < transitionTime)
        {
            
            timer += Time.unscaledDeltaTime;
            temp.a = Mathf.Lerp(from,to,timer/transitionTime);
            fading.color = temp;
            yield return null;
        }

        temp.a = to;
        fading.color = temp;
    }

    public void startGame()
    {
        
        SceneManager.LoadScene("Level_1");

    }
}
