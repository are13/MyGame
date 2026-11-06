
using UnityEngine;
public class Level1Key : Item, IUsable
{
    
    public Level1Key(string id,string gameObject,string sprite = null,string displayName = "untitled" ,string description = "",int maxStackNum = 64 , ItemTypes itemType = ItemTypes.MISC):base(id,gameObject,sprite,displayName,description,maxStackNum,itemType)
    {
    }

    public void use(LivingEntity user, ItemStack stack)
    {
        
        if(user is Player player)
        {
            
            player.addItem(new ItemStack(Items.ICED_FROG,60));
        }
    }
}