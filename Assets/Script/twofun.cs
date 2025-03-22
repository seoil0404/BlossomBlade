using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twofun : MonoBehaviour
{
    public GameObject tr;
    Transform tf;
    float i;

    private void Awake()
    {
        tf = tr.GetComponent<Transform>();
        i = -10;
    }

    // Update is called once per frame
    void Update()
    {
        float x;
        x = i;
        float y;
        y = 1 / x;
        i += 0.05f;
        tf.position = new Vector3(0, y, x);
    }
}
