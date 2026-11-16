

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObj : MonoBehaviour
{
    
    public static PlayerObj Instance;
    public Player player;

    void Awake()
    {
        
        if(Instance == null)
        {
            
            Instance = this;
        }else if(Instance != this)
        {
            
            Destroy(this.gameObject);
            return;
        }

        
    }

    void Update()
    {
        
        if(player.hasDie && Keyboard.current.rKey.wasPressedThisFrame)
        {
            
            player.gameObject.SetActive(true);
            player.titleInfo.endShow();
            player.restart();
        }
    }
}