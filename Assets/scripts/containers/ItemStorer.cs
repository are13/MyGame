

using System.Collections.Generic;
using UnityEngine.Events;

public class ItemStorer
{

    public UnityEvent<int> onContainerChanged;
    readonly List<ItemStack> content;
    int maxCount;

    public ItemStorer(int maxCount = 16)
    {
        this.maxCount = maxCount;
        content = new List<ItemStack>(maxCount);
        for(int i=0 ; i<maxCount ; i++) content.Add(null);
    }

    public ItemStack getItem(int idx)
    {

        if (isVaildSlot(idx))
        {

            return content[idx];
        }
        return null;
    }

    public void setItem(int idx, ItemStack stack)
    {

        if (isVaildSlot(idx)) content[idx] = stack;
    }

    public bool addItem(ItemStack stack)
    {

        bool isAllPicked = false;
        List<int> changed = new List<int>();
        for (int i = 0; i < maxCount; i++)
        {
            if (content[i] == null)
            {

                content[i] = stack;
                changed.Add(i);
                isAllPicked = true;
                changed.Add(i);
                break;
            }
            else if (stack.item.Equals(content[i].item))
            {

                int added = content[i].amount + stack.amount;
                if (added > content[i].item.maxStackNum)
                {

                    int d = content[i].item.maxStackNum - content[i].amount;
                    if (d > 0) changed.Add(i);
                    stack.amount -= d;
                    content[i].amount = content[i].item.maxStackNum;
                }
                else
                {
                    content[i].amount += stack.amount;
                    isAllPicked = true;
                    changed.Add(i);
                    break;
                }
            }
        }

        notify(changed);
        return isAllPicked;
    }

    public bool isVaildSlot(int idx) => idx >= 0 && idx < content.Count;

    void notify(List<int> ints)
    {
        foreach (var changes in ints)
        {
            onContainerChanged?.Invoke(changes);
        }
    }
}