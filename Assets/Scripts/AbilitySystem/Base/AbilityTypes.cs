using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 技能类型
/// </summary>
public enum EAbilityType
{
    EAT_PassiveAblity = 0,      //被动
    EAT_GeneralAbility = 1,     //普通
    EAT_ChannelAbility = 2,     //引导
    EAT_ToggleAbility = 4,      //开关
};
/// <summary>
/// 目标类型
/// </summary>
public enum ETargetType
{
    ETT_None,
    ETT_Target,
    ETT_Ground,
};
/// <summary>
/// 持续时间策略
/// </summary>
public enum EDurationPolicy
{
    EDP_Instant,            //立即
    EDP_Infinite,           //永久
    EDP_HasDuration,        //一定时间
}
/// <summary>
/// 属性类型
/// </summary>
public enum EAttributeType
{
    AT_None,
    AT_Health,
    AT_Mana,
    AT_Coin,
}
/// <summary>
/// 修改器选项
/// </summary>
public enum EBuffModifierOption
{
    EBMO_Add,               //+
    EBMO_Mul,               //*
    EBMO_Divide,            //'/'
    EBMO_Override,          //=
}
/// <summary>
/// 修改器类型
/// </summary>
public enum EBuffModifierType
{
    EBMT_All,
    EBMT_Base,
    EBMT_Extra,
    EBMT_Extra_AcceptMul,
}

public enum EOperationKey
{
    EOK_Down,
    EOK_Up,
};

public enum EOverlapType
{
    Sphere,     //圆      x = y = z = 2*radius
    Box,        //方形    x = width， z = length， y = height
    Cylinder,   //圆柱    x = z = 2*raidus， y = height
    Sector,     //扇形    x = z = 2*raidus， y = angle°
    Triangle,   //三角形  x = z = edgeLength， y = height (只适用于等腰三角形)
};

public struct FAbilityData
{
    // ability
    public EAbilityType abilityType;

    // 选择方式
    public ETargetType targetType;
    public KeyCode selectKeyCode;
    public KeyCode unSelectKeyCode;
    public ESelectTarget selectTargetStatus;

    // 施法范围
    public float spellRadius;
    public EOverlapType spellOverlapType;
    public Vector3 spellRange;
    // OnSpell回调时间点
    public float castPoint;
    public float totalTime;

    public float channelStartTime;
    public float channelInterval;
    public float channelEndTime;
};

public struct FAbilityBuffData
{
    public List<AbilityBuffModifiers> modifiers;
    public EDurationPolicy durationPolicy;

    public List<float> durations;
    public List<float> intervals;

    public int maxStack;
    public int stack;

    public bool bActiveFirst;
};