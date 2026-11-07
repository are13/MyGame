

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleInfo : MonoBehaviour
{
    
    public Image image;
    public TextMeshProUGUI text;
    public bool isShown;
    public float showTime = 10;
    float showTimer;


    void Update()
    {
        if(isShown && showTimer <= 0)
        {
            isShown = false;
            gameObject.SetActive(false);
        }else showTimer-=Time.deltaTime;
    }

    public void setShow(String info,float duration=0f)
    {

        if(duration == -1) showTimer = 100000f;
        else showTimer = showTime;
        text.text = info;

        gameObject.SetActive(true);
        image.gameObject.SetActive(false);
    }

    public void endShow()
    {
        
        gameObject.SetActive(false);
    }
}