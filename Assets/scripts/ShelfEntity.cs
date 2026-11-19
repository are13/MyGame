



using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestEntity : MonoBehaviour, IContainer, IInteractable
{

    public Transform containerLayout;
    public GameObject slotPrefab;
    public ChestPanel panel;
    public int maxCount;
    public int old_selected = 0;
    public int id;
    bool hasInit;
    ItemStorer itemStorer;
    List<Slot> slots = new List<Slot>();
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

        containerLayout.gameObject.SetActive(true);
        
        if (!hasInit)
        {
            for (int i = 0; i < maxCount; i++)
            {

                slots.Add(Instantiate(slotPrefab, containerLayout.transform).GetComponent<Slot>());
            }

            slots[old_selected].select(true);
            hasInit = true;
        }
    }

    public void updateSelected(int idx)
    {

        if (idx == old_selected) return;
        slots[old_selected].select(false);
        slots[idx].select(true);

        old_selected = idx;
    }

    public void closeContainer()
    {

        containerLayout.gameObject.SetActive(false);
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