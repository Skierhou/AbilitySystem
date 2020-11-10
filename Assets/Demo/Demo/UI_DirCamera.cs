using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DirCamera : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
        transform.rotation = Camera.main.transform.rotation;
    }
}
