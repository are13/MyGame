using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogNode
{
    
    public String id;
    [SerializeReference,SerializeReferenceSelector] public DialogNode next;
    public Vector2 position;
}

[Serializable]
public class DialogText : DialogNode
{
    
    public String speaker; 
    public String text;
}

[Serializable]
public class ActionNode : DialogNode
{
    
    [SerializeReference, SerializeReferenceSelector] public List<DialogAction> actions;
}

[Serializable]
public class EndNode : DialogNode
{
    

}

[Serializable]
public class OptionNode : DialogNode
{
    

}