

using System;
using TMPro;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    
    public int maxShown = 8;
    // public TextMeshProUGUI preFabText;
    public static float WIGTH = 507;
    public static float HEIGHT = 20;
    public static int FONT_SIZE = 20;

    public static TextMeshProUGUI generate()
    {
        var temp = new GameObject("",typeof(RectTransform),typeof(TextMeshProUGUI));
        temp.GetComponent<TextMeshProUGUI>().fontSize = FONT_SIZE;
        temp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,HEIGHT);
        return temp.GetComponent<TextMeshProUGUI>();
    }

    public void addInfo(String text)
    {
        var temp = generate();
        temp.text = text;
        addInfo(temp);
    }

    public void addInfo(TextMeshProUGUI textMeshPro)
    {
        
        textMeshPro.transform.SetParent(transform,false);
        if(transform.childCount > maxShown) DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public void setActive(bool act)
    {
        
        gameObject.SetActive(act);
    }
}