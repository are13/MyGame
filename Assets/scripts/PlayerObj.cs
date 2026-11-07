

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObj : MonoBehaviour
{
    
    public Player player;

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