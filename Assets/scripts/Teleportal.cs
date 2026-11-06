

using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleportal : TriggerEntity
{

    public override void trigger(GameObject gameObject)
    {
        base.trigger(gameObject);
        if(gameObject.GetComponent<LivingEntity>() is Player player)
        {
            
            SceneManager.LoadScene("Level_2");
        }
    }
}