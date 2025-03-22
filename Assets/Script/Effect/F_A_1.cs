using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_A_1 : MonoBehaviour
{
    public GameObject ob;
    private GameObject player;
    public GameObject obj; //Name : Col
    public ParticleSystem par;
    public GameObject parob;
    public GameObject gamemanager;
    public bool fb; //true = front, false = back

    private void Awake()
    {
        gamemanager = GameObject.Find("Game_Manager");
        gamemanager.GetComponent<Game_Manager>().ActiveDelay(0.5f, ob);
    }

    private void OnEnable()
    {
        Before();
    }

    public void Before()
    {
        player = GameObject.Find("Player");
        if (fb == true)
        {
            parob.transform.localPosition = new Vector3(-46.1500015f, -2.7308073f, -9.52999973f);
            parob.transform.localRotation = Quaternion.Euler(-163.37f, 0, 0f);
            obj.transform.rotation = Quaternion.Euler(21, 0, 0);
            transform.position = new Vector3(47.3f, 4, 20) + player.transform.position;
        }
        else
        {
            parob.transform.localPosition = new Vector3(-46.1500015f, -2.7308073f, -1.95000005f);
            parob.transform.localRotation = Quaternion.Euler(-163.37f, 180, 0f);
            obj.transform.rotation = Quaternion.Euler(21, 180, 0);
            transform.position = new Vector3(47.3f, 4, -7f) + player.transform.position;
        }
        StartCoroutine(Del());
    }


    IEnumerator Del()
    {
        yield return new WaitForSeconds(0.1f);
        par.Play();
        yield return new WaitForSeconds(3f);
        Destroy(ob);
    }
}
