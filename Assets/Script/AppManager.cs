using System;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(480,800,true);
        MenuListManager.Instance.SetView(false);
        SaveUIManager.Instance.SetView(false);
        GameObject.Find("EventSystem").AddComponent<SoundManager>();
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
