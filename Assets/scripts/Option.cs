using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    
    public Image iconImg;
    public Image isSelected;
    public TextMeshProUGUI text;
    public Action<Player> action;

    public Option init(Action<Player> action,String text="test",Image image = null)
    {
        
        this.action = action;
        // ***********changed**************
        if (this.text != null) this.text.text = text;
        // ***********changed**************
        if(image != null) iconImg = image;

        return this;
    }

    public void select(bool b)
    {
        
        // ***********changed**************
        if (isSelected != null) isSelected.gameObject.SetActive(b);
        // ***********changed**************
    }

    // public void updateDisplay()
    // {
        
    //     iconImg = Resources.Load<Image>(this.icon);
    // }

    public virtual void onClick(Player player)
    {
        
        action?.Invoke(player);
    }
}