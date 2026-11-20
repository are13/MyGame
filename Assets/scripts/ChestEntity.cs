



using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestEntity : MonoBehaviour, IContainer, IInteractable
{

    public ChestPanel panel;
    // public int old_selected = 0;
    public int id;
    bool hasInit;
    int maxCount;
    ItemStorer itemStorer;
    // List<ItemStack> items = new List<ItemStack>();

    void Start()
    {
        itemStorer = new ItemStorer(maxCount);
    }
    public String getDisplayName()
    {

        return "Shelf";
    }

    public int getId()
    {
        
        return id;
    }

    public void interact(Player player)
    {

        GameManager.manager.containerManager.open(this);
    }

    public void openContainer()
    {

        panel.gameObject.SetActive(true);
        panel.init(maxCount);
        
    }

    public void updateSelected(int idx)
    {
        panel.updateSelected(idx);
    }

    public void closeContainer()
    {

        panel.gameObject.SetActive(false);
    }

    // public void takeItem(Player player)
    // {

    //     if (slots[old_selected] != null && player.GetInventory().Count < player.inventorySize - 1)
    //     {

    //         player.addItem(items[old_selected]);
    //         items.RemoveAt(old_selected);
    //     }
    // }

    // public void putItem(Player player, int idx)
    // {

    //     var itemStack = player.GetInventory()[idx];
    //     if (itemStack != null && items.Count < maxCount - 1)
    //     {

    //         items[old_selected] = itemStack;
    //         player.removeItemWithoutDestory(idx);
    //     }
    // }
}