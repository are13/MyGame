using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : MonoBehaviour, IInteractable
{

    public int id;
    public String disPlayname;
    // ***********added**************
    [SerializeReference] public DialogContent dialogs;
    // ***********added**************
    protected bool hasInit = false;

    public virtual void init()
    {
        
    }

    public String getDisplayName()
    {
        
        return disPlayname;
    }
    public int getId()
    {
        
        return id;
    }

    public virtual void interact(Player player)
    {
        if(!hasInit) init();

        GameManager.manager.dialogManager.startDialog(dialogs,disPlayname);
    }
}