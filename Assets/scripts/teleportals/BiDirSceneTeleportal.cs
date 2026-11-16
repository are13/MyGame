
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BiDirSceneTeleportal : TriggerEntity
{
    
    public String from;
    public String to;

    public override void trigger(GameObject gameObject)
    {
        base.trigger(gameObject);
        if (SceneManager.GetActiveScene().name == from)
        {
            
            GameManager.manager.transformScene(to);
        }
        else
        {
            
            GameManager.manager.transformScene(from);
        }
    }
}