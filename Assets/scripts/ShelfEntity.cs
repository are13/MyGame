



using System.Collections.Generic;
using UnityEngine;

public class ShelfEntity : NPCEntity,IContainer
{
    
    public Transform containerLayout;
    public GameObject slotPrefab;
    public int maxCount;
    public int old_selected=0;

    List<Slot> slots = new List<Slot>();
    List<ItemStack> items = new List<ItemStack>();

    public override void init()
    {
        
        for (int i = 0; i < maxCount; i++)
        {
            
            slots.Add(Instantiate(slotPrefab,containerLayout).GetComponent<Slot>());
        }
    }

    // public override void interact(Player player)
    // {
        

    // }

    public void openContainer()
    {
        
        containerLayout.gameObject.SetActive(true);
    }

        public void updateSelected(int idx)
    {

        if (idx == old_selected) return;
        slots[old_selected].select(false);
        slots[idx].select(true);

        old_selected = idx;
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

        if ( slots[old_selected] != null && player.GetInventory().Count < player.inventorySize-1)
        {
            
            player.addItem(items[old_selected]);
            items.RemoveAt(old_selected);
        }
    }

    public void putItem(Player player , int idx)
    {
        
        var itemStack = player.GetInventory()[idx];
        if(itemStack != null && items.Count < maxCount - 1)
        {
            
            items[old_selected] = itemStack;
            player.removeItemWithoutDestory(idx);
        }
    }
}