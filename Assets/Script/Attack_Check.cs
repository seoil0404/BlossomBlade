using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Check : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.name == "Enemy") Debug.Log(damage);
    }
}
