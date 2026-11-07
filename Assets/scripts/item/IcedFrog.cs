using UnityEngine;

public class IcedFrog : Item
{

    public IcedFrog(string id,GameObject gameObject,Sprite sprite = null,string displayName = "untitled" ,string description = "",int maxStackNum = 64 , ItemTypes itemType = ItemTypes.MISC):base(id,gameObject,sprite,displayName,description,maxStackNum,itemType)
    {
    }
    
}