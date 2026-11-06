using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public int hotBarSize;
    public int inventorySize;
    public int old_selected = 0;

    // public Transform inventory;
    public Transform hotBar;

    public GameObject slot;

    List<Slot> slots = new List<Slot>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void init(int size)
    {

        hotBarSize = (size > 8) ? 8 : size;
        inventorySize = size - hotBarSize;


        for (int i = 0; i < hotBarSize; i++)
        {

            slots.Add(Instantiate(slot, hotBar.transform).GetComponent<Slot>());
        }

        for (int i = 0; i < inventorySize; i++)
        {

            slots.Add(Instantiate(slot, transform).GetComponent<Slot>());
        }

        slots[old_selected].select(true);

    }

    public void updateSelected(int idx)
    {

        if (idx == old_selected) return;
        slots[old_selected].select(false);
        slots[idx].select(true);

        old_selected = idx;
    }

    public void updateDisplay(List<ItemStack> itemStacks, List<int> changes)
    {

        if (changes != null)
        {

            for (int i = 0; i < changes.Count; i++)
            {
                if(slots[changes[i]] != null)
                {
                    
                    slots[changes[i]].updateDisplay(itemStacks[changes[i]]);
                }
            }
        }
        else
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


    }

    // Update is called once per frame
    void Update()
    {

    }
}
