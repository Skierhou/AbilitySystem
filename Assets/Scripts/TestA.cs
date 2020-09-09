
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestA : Test
{
    float testa_float;

    void Start()
    {
        this.ReadConfig();
        Debug.Log(test_Float);
        Debug.Log(test_Int);
        Debug.Log(test_String);
        Debug.Log(test_Struct.test_Float + " " + test_Struct.test_Int + " " + test_Struct.test_String);
        Debug.Log(test_List.Count);
        Debug.Log(test_StructList[0].test_String + " " + test_StructList[1].test_Float);
        Debug.Log(testa_float);
    }
}