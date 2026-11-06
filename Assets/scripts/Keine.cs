
using UnityEngine;
public class Keine : NPCEntity
{

    public override void init()
    {
        base.init();
        options[0].init((player)=> print("aaaa"),"chat");
        options[1].init((player)=>player.addItem(new ItemStack(Items.LEVEL1_KEY,1)),"next level");
        hasInit = true;
    }
}