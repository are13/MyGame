using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : MonoBehaviour, IInteractable
{

    public string id;
    // ***********added**************
    [SerializeReference] public DialogContent dialogs;
    // ***********added**************
    protected bool hasInit = false;

    public virtual void init()
    {
        
    }

    public string getId()
    {
        
        return id;
    }

    public virtual void interact(Player player)
    {
        if(!hasInit) init();

        GameManager.manager.dialogManager.startDialog(dialogs,id);
    }
}