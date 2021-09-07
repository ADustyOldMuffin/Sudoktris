using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevPreload : SingletonBehavior<DevPreload>
{
    protected override void Awake()
    {
        base.Awake();
        
        // Load the preload scene and then reload
        if (GameObject.Find("__app") != null)
            return;
        
        SceneManager.LoadScene("_Preload");
    }
}