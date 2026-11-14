

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GiveItem",menuName ="Game/Action/Give Item")]
public class DialogGivingItemAction : DialogActionData
{
    
    public LootTable items;
    public override void execute(Player player)
    {
        
        // ***********changed**************
        if (player == null || items == null || items.loots == null) return;

        foreach (LootData item in items.loots)
        {
            if (item == null || item.item == null) continue;
            if (item.chance < 1f && Random.value > item.chance) continue;
            if (!ItemDataInitializer.read.TryGetValue(item.item.id, out Item data)) continue;

            int min = Mathf.Max(1, item.minAmount);
            int max = Mathf.Max(min, item.maxAmount);
            player.addItem(new ItemStack(data, Random.Range(min, max + 1)));
        }
        // ***********changed**************
    }
}