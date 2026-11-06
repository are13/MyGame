
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
            var temp = InfoText.generate();
            temp.color = Color.green;
            temp.text = "You win!";
            player.infoText.addInfo(temp);

            var temp1 = InfoText.generate();
            temp1.color = Color.blue;
            temp1.text = "Next level is coming...";
            player.infoText.addInfo(temp1);

            var temp2 = InfoText.generate();
            temp2.color = Color.blue;
            temp2.text = "w";
            player.infoText.addInfo(temp2);

            var temp3 = InfoText.generate();
            temp3.color = Color.blue;
            temp3.text = "Next level is coming...";
            player.infoText.addInfo(temp3);

            var temp4 = InfoText.generate();
            temp4.color = Color.blue;
            temp4.text = "Next level is coming...";
            player.infoText.addInfo(temp4);
        }
    }
}