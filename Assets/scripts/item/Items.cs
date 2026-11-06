
public class Items
{
    
    public static string path_prefab = "sprite/prefabs/items/";
    public static string path_icon = "item/icon/";
    public static Item ICED_FROG = new IcedFrog("iced_frog",path_prefab+"iced_frog",path_icon+"iced_frog");
    public static Item SCROLL_SPEED = new ScrollSpeed("scroll_speed",path_prefab+"scroll_speed",path_icon+"scroll","Speed Scroll","use to increase your move speed",1,ItemTypes.MISC);
    public static Item LEVEL1_KEY = new Level1Key("level1_key",path_prefab+"level1_key",path_icon+"level1_key","Level 1 key","Use it to get to the next level.",16,ItemTypes.MISC);

    public static void init()
    {
        
        
    }
}