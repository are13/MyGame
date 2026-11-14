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
        this.text.text = text;
        if(image != null) iconImg = image;

        return this;
    }

    public void select(bool b)
    {
        
        isSelected.gameObject.SetActive(b);
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