using System;
using Unity.VisualScripting;
using UnityEngine;

public class Item
{
    

    public string id;
    public string displayName;
    [TextArea] public string description;
    public int maxStackNum;
    public string icon;
    public ItemTypes type;
    public string preFab;

    public Item(string id,string gameObject,string sprite = null,string displayName = "untitled" ,string description = "",int maxStackNum = 64 , ItemTypes itemType = ItemTypes.MISC)
    {
        
        this.id = id;
        this.displayName = displayName;
        this.description = description;
        this.maxStackNum = maxStackNum;
        this.icon = sprite;
        this.type = itemType;
        this.preFab = gameObject;
    }

    public virtual void create()
    {
        

    }

    public virtual void Update()
    {
        

    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if(obj == null) return false;
        return ((Item)obj).id == id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }
}

public enum ItemTypes
{
    
    MISC,FOOD,ARMOR,WEAPON
}