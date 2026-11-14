
using UnityEngine;
public class Keine : NPCEntity
{

    public override void init()
    {
        base.init();
        // **************changed*****************
        // Legacy choices are optional once a DialogNode is authored for this NPC.
        if (options != null && options.Count > 0 && options[0] != null)
            options[0].init((player) => print("aaaa"), "chat");
        if (options != null && options.Count > 1 && options[1] != null)
            options[1].init((player) => player.addItem(new ItemStack(Items.LEVEL1_KEY, 1)), "next level");
        // **************changed*****************
        hasInit = true;
    }
}