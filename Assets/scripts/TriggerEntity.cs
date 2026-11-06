using UnityEngine;

public class TriggerEntity : MonoBehaviour
{
    
    public virtual void trigger(GameObject gameObject)
    {
        

    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        
        trigger(collider.gameObject);
    }
}