
using UnityEngine;
public class Keine : NPCEntity
{

    public override void init()
    {
        base.init();
        // if (options != null && options.Count > 0 && options[0] != null)
        //     options[0].init((player) => print("aaaa"), "chat");
        // if (options != null && options.Count > 1 && options[1] != null)
        //     options[1].init((player) => player.addItem(new ItemStack(Items.LEVEL1_KEY, 1)), "next level");
        hasInit = true;
    }
}