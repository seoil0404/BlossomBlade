using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camere_Follow : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(-7.5f, 2.4f, 0) + player.position, 10*Time.deltaTime);
    }
}
