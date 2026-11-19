using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class DialogAction
{
    
    public abstract void execute(Player player, GameObject gameObject=null);
}

[Serializable]
public class GiveItemAction : DialogAction
{

    public List<LootData> items;
    public override void execute(Player player,GameObject gameObject=null)
    {
        
        foreach (var item in items)
        {
            
            player.addItem(new ItemStack(ItemDataInitializer.read[item.item.id],item.minAmount));
        }
    }
}

[Serializable]
public class openContainerAction : DialogAction
{

    public override void execute(Player player, GameObject gameObject)
    {
        
        gameObject.GetComponentInChildren<IContainer>().openContainer();
    }

}