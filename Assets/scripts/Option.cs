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
    public int id;

    public Option init(Action<Player> action,String text="test",Image image = null)
    {
        
        this.action = action;
        this.text.text = text;
        if(image != null) iconImg = image;

        return this;
    }

    public void setId(int id)
    {
        
        this.id = id;
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
    public override bool Equals(object obj)
    {
        if(obj is Option option)
        {
            
            return this.id == option.id;
        }
        return false;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        return id;
    }
}