

using System.Collections.Generic;

public class ChestPanel : ContainerPanel
{
    public int containerSize;
    List<Slot> slots;

    public override void init(int size)
    {

        for (int i = 0; i < containerSize; i++)
        {

            slots.Add(Instantiate(slot, transform).GetComponent<Slot>());
        }

        slots[old_selected].select(true);

    }
}