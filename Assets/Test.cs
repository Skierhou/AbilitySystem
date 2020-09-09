using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

public struct FTest
{
    public float test_Float;
    public int test_Int;
    public string test_String;
};

public enum ETest
{
    None,
    Test1,
};

[Config("Default")]
public class Test : MonoBehaviour
{
    [Config]
    protected float test_Float;
    [Config]
    protected int test_Int;
    [Config]
    protected string test_String;
    [Config]
    protected FTest test_Struct;
    [Config]
    protected List<string> test_List;
    [Config]
    protected List<FTest> test_StructList;

    void Start()
    {
        this.ReadConfig();
        Debug.Log(test_Float);
        Debug.Log(test_Int);
        Debug.Log(test_String);
        Debug.Log(test_Struct.test_Float + " " + test_Struct.test_Int + " " + test_Struct.test_String);
        Debug.Log(test_List.Count);
        Debug.Log(test_StructList[0].test_String + " " + test_StructList[1].test_Float);

    }


    // Update is called once per frame
    AbilityFire abilityFire;
    AbilityIce abilityIce;
    AbilitySystemComponent comp;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            abilityFire.abilityData.abilityType = EAbilityType.EAT_PassiveAblity;
            comp.TryActivateAbilityByTag("Ability.Fire");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            abilityFire.abilityData.abilityType = EAbilityType.EAT_GeneralAbility;
            comp.TryActivateAbilityByTag("Ability.Fire");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            abilityFire.abilityData.abilityType = EAbilityType.EAT_ChannelAbility;
            comp.TryActivateAbilityByTag("Ability.Fire");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            comp.TryActivateAbilityByTag("Ability.Ice");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            abilityFire.EndAbility();
            abilityIce.EndAbility();
        }
    }
}
