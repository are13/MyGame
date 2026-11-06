
using System;
using UnityEngine;
public class Level1Key : Item, IUsable
{
    
    public String portal = "sprite/prefabs/trigger/teleportal";
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

            GameManager.Instantiate(Resources.Load<GameObject>(portal),new Vector3(player.transform.position.x,player.transform.position.y-3,player.transform.position.z),Quaternion.identity);
        }
    }
}