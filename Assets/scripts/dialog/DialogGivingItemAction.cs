

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GiveItem",menuName ="Game/Action/Give Item")]
public class DialogGivingItemAction : DialogActionData
{
    
    public LootTable items;
    public override void execute(Player player)
    {
        
        foreach (LootData item in items.loots)
        {
            player.addItem(new ItemStack(ItemDataInitializer.read[item.item.id],1));
        }
    }
}