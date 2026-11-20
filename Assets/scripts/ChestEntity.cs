



using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestEntity : MonoBehaviour, IContainer, IInteractable
{

    public ChestPanel panel;
    // public int old_selected = 0;
    public int id;
    public int maxCount;
    bool hasInit;
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
        panel.init(maxCount,itemStorer);
    }

    public void updateSelected(int idx)
    {
        panel.updateSelected(idx);
    }

    public void updateClicked(int idx)
    {
        
        GameManager.manager.dragManager.startDragging(itemStorer,idx);
    }

    public void closeContainer()
    {

        panel.gameObject.SetActive(false);
    }
}