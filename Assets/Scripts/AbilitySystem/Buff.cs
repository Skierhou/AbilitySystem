using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuffType
{
    EBT_None,
};
public enum EBuffTag
{
    EBT_Metal,
    EBT_Wood,
    EBT_Water,
    EBT_Fire,
    EBT_Earth,
};

public class Buff
{
    private EBuffType buffType;
    private EBuffTag buffTag;           //自身标签
    private EBuffTag buffImmuneTag;     //免疫标签

    public bool bNoCaster;
    private GameObject caster;
    private GameObject target;
    private Ability ability;

    private int layer;
    private int level;
    private float druation;

    public object content;

    public GameObject Caster { get => bNoCaster ? null : caster; set => caster = value; }
}
