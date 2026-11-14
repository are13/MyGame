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

    public List<Option> interact(Player player)
    {
        if(!hasInit) init();

        // ***********added**************
        DialogNode node = resolveDialogNode();
        if (node != null)
        {
            DialogController controller = DialogController.Ensure();
            if (controller != null && controller.startDialog(player, node, this))
            {
                // The controller now owns the option list.
                return null;
            }
        }
        // ***********added**************

        return options;
    }

    // ***********added**************
    protected DialogNode resolveDialogNode()
    {
        if (dialog != null) return dialog;

        string key = string.IsNullOrEmpty(id) ? gameObject.name : id;
        DialogNode node = DialogInitializer.find(key);
        if (node == null) node = DialogInitializer.find(GetType().Name);
        if (node == null) node = DialogInitializer.find(gameObject.name);
        return node;
    }
    // ***********added**************

    // public void showOptions(Player player, bool isShow)
    // {

    //     options.gameObject.SetActive(isShow);
    //     options.updateSelected(0);
    //     isShowing = isShow;
    //     player.isLookingOptions = isShow;
    // }

    // public void updateSelect(Player player, Vector2 dSelect)
    // {

    //     print("invoked");
    //     if (player.isLookingOptions && isShowing)
    //     {
    //         selected = (int)Mathf.Clamp(selected + dSelect.y, 0, maxIdx);
    //         options.updateSelected(selected);
    //     }
            
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {

        // print("entered");
        if (collision.gameObject.GetComponent<LivingEntity>() is Player player)
        {

            // showOptions(player, true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<LivingEntity>() is Player player)
        {

            // showOptions(player, false);
        }
    }
}