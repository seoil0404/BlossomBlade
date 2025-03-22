using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public GameObject player;
    public GameObject canvas;
    public GameObject trail;
    public GameObject cam;
    public GameObject eventmanager;
    public GameObject gamemanager;
    public GameObject scenemanager;
    private void Awake()
    {
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(trail);
        DontDestroyOnLoad(cam);
        DontDestroyOnLoad(eventmanager);
        DontDestroyOnLoad(gamemanager);
        DontDestroyOnLoad(scenemanager);
    }

    public void SceneMove(string name)
    {
        SceneManager.LoadScene(name);
        if(name == "stage1")
        {
            player.transform.position = new Vector3(0, 0.2639999f, 12);
        }
    }
}
