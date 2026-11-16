

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoDirSceneTeleportal : TriggerEntity
{
    public String to;

    public override void trigger(GameObject gameObject)
    {
        base.trigger(gameObject);
        GameManager.manager.transformScene(to);
    }
}