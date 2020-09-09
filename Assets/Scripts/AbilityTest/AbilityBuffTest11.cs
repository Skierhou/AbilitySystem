using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBuffTest11 : AbilityFire
{
    public AbilityBuffTest11(FAbilityData data, AbilitySystemComponent abilitySystemComponent) : base(data, abilitySystemComponent)
    {
    }

    public float test_float;
    public int test_int;
    public byte test_byte;
    public string test_str;
    public char test_char;
    public GameObject test_go;
    public ETest test;
    public List<int> test_list;
}
