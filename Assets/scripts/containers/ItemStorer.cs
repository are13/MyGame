using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemStorer
{

    public UnityEvent<int> onContainerChanged = new UnityEvent<int>();

    readonly List<ItemStack> content;

    public int maxCount => content.Count;

    public ItemStorer(int maxCount = 16)
    {
        content = new List<ItemStack>(maxCount);
        for (int i = 0; i < maxCount; i++) content.Add(null);
    }

    public bool isVaildSlot(int idx) => idx >= 0 && idx < content.Count;

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

        if (!isVaildSlot(idx)) return;

        content[idx] = stack;
        notify(idx);
    }

    public void notify(int idx) => onContainerChanged?.Invoke(idx);

    public bool addItem(ItemStack stack)
    {

        if (stack == null || stack.amount <= 0) return true;

        for (int i = 0; i < content.Count && stack.amount > 0; i++)
        {
            var slot = content[i];
            if (slot == null || !slot.item.Equals(stack.item)) continue;
            if (slot.amount >= slot.item.maxStackNum) continue;

            int move = Mathf.Min(slot.item.maxStackNum - slot.amount, stack.amount);
            slot.amount += move;
            stack.amount -= move;
            notify(i);
        }

        for (int i = 0; i < content.Count && stack.amount > 0; i++)
        {
            if (content[i] != null) continue;

            int move = Mathf.Min(stack.item.maxStackNum, stack.amount);
            content[i] = new ItemStack(stack.item, move);
            stack.amount -= move;
            notify(i);
        }

        return stack.amount <= 0;
    }
}
