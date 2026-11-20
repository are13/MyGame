

using System.Collections.Generic;
using UnityEngine;

public class ChestPanel : ContainerPanel
{
    int containerSize;
    // public Transform containerLayout;
    List<Slot> slots;
    
    public override void init(int size , ItemStorer itemStorer)
    {

        if (!hasInit)
        {

            containerSize = size;
            slots = new List<Slot>(containerSize);
            for (int i = 0; i < containerSize; i++)
            {

                var temp = Instantiate(slotPrefab, transform).GetComponent<Slot>();
                temp.idx = i;
                temp.notifySelect = updateSelected;
                temp.notifyClick = click;
                temp.bind(itemStorer,i);
                slots.Add(temp);
            }

            slots[old_selected].select(true);
            hasInit = true;
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

    public void click(int idx)
    {
        
        onSelectionClicked?.Invoke(idx);
    }
}