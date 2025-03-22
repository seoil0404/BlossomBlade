using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public GameObject[] attack_button; //attack button
    public GameObject Player;

    public void ActiveDelay(float t, GameObject obj)
    {
        StartCoroutine(activedelay_(t, obj));
    }

    IEnumerator activedelay_(float t, GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(t);
        obj.SetActive(true);
    }
}
