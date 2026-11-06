using UnityEngine;

public class follow : MonoBehaviour
{

    public Transform player;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    // public Camera mainCam;

    void LateUpdate()
    {
        
        if(player == null) return;

        Vector3 targetPos = Vector3.Lerp(transform.position,player.position +offset,smoothSpeed*Time.deltaTime);
    
        // float w = mainCam.orthographicSize*mainCam.aspect;
        // if(borderL != null && borderR != null && (targetPos.x - w <= borderL.position.x || targetPos.x + w >= borderR.position.x))
        // {
        //     return;
        // }else 
        transform.position = targetPos;
        // print(targetPos); 
    }
}
