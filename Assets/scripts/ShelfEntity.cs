



using System;
using System.Collections.Generic;
using UnityEngine;

public class ShelfEntity : MonoBehaviour, IContainer, IInteractable
{

    public Transform containerLayout;
    public GameObject slotPrefab;
    public int maxCount;
    public int old_selected = 0;
    bool hasInit;

    List<Slot> slots = new List<Slot>();
    List<ItemStack> items = new List<ItemStack>();

    public String getId()
    {

        return "Shelf";
    }

    public void interact(Player player)
    {

        GameManager.manager.containerManager.open(this);
    }

    // public override void interact(Player player)
    // {


    // }

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

    public void updateDisplay(List<ItemStack> itemStacks)
    {


        for (int i = 0; i < slots.Count; i++)
        {
            if (i < itemStacks.Count)
            {

                slots[i].updateDisplay(itemStacks[i]);
            }
            else slots[i].updateDisplay(null);
        }
    }

    public void takeItem(Player player)
    {

        if (slots[old_selected] != null && player.GetInventory().Count < player.inventorySize - 1)
        {

            player.addItem(items[old_selected]);
            items.RemoveAt(old_selected);
        }
    }

    public void putItem(Player player, int idx)
    {

        var itemStack = player.GetInventory()[idx];
        if (itemStack != null && items.Count < maxCount - 1)
        {

            items[old_selected] = itemStack;
            player.removeItemWithoutDestory(idx);
        }
    }
}