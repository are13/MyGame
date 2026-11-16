using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorKosuzuHomeEntity : TriggerEntity
{

    public override void trigger(GameObject gameObject)
    {
        base.trigger(gameObject);
        if(gameObject.TryGetComponent<Player>(out var player))
        {
            
            if(SceneManager.GetActiveScene().name == "KosuzuHome")
            {

                SceneManager.LoadScene("CottageStreet");
            }
            else
            {
                
                SceneManager.LoadScene("KosuzuHome");
            }
        }
    }
}
