

public class ScrollSpeed : Item,IUsable
{
    
    public ScrollSpeed(string id,string gameObject,string sprite = null,string displayName = "untitled" ,string description = "",int maxStackNum = 64 , ItemTypes itemType = ItemTypes.MISC):base(id,gameObject,sprite,displayName,description,maxStackNum,itemType)
    {
    }

    
    public void use(LivingEntity user, ItemStack stack)
    {

        if(user is Player player)
        {
            
            player.setSpeed(player.speed + 5f);
        }
    }


}