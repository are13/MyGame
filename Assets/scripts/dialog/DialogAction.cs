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
public class DropItemAction : DialogAction
{

    public List<LootData> itemDatas;
    public List<Vector2> pos;
    public bool isHere = false;
    public override void execute(Player player, GameObject gameObject = null)
    {
        
        for (int i = 0; i < itemDatas.Count; i++)
        {
            GameManager.manager.spawnItem(itemDatas[i],isHere?player.transform.position:pos[i]);
        }
        
    }
}