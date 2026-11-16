using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : MonoBehaviour, IInteractable
{

    public string id;
    public List<Option> options;
    // ***********added**************
    [Tooltip("Dialog started when the player interacts with this NPC. Left empty the NPC looks the node up in Resources/dialog by id/name.")]
    public DialogNode dialog;
    // ***********added**************
    protected bool hasInit = false;

    public virtual void init()
    {
        
        
    }

    public string getId()
    {
        
        return id;
    }

    public virtual DialogNode interact(Player player)
    {
        if(!hasInit) init();

        // // ***********added**************
        // DialogNode node = resolveDialogNode();
        // if (node != null)
        // {
        //     DialogController controller = DialogController.Ensure();
        //     if (controller != null && controller.startDialog(player, node, this))
        //     {
        //         // The controller now owns the option list.
        //         return null;
        //     }
        // }

        return dialog;
    }
    protected DialogNode resolveDialogNode()
    {
        if (dialog != null) return dialog;

        string key = string.IsNullOrEmpty(id) ? gameObject.name : id;
        DialogNode node = DialogInitializer.find(key);
        if (node == null) node = DialogInitializer.find(GetType().Name);
        if (node == null) node = DialogInitializer.find(gameObject.name);
        return node;
    }
}