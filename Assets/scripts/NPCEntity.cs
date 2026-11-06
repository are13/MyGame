using System.Collections.Generic;
using UnityEngine;

public abstract class NPCEntity : MonoBehaviour, IInteractable
{

    public List<Option> options;
    protected bool hasInit = false;

    public virtual void init()
    {
        
        
    }

    public List<Option> interact(Player player)
    {
        if(!hasInit) init();
        return options;
    }

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