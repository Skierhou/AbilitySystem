
/// <summary>
/// 技能类型
/// </summary>
public enum EAbilityType
{
    EAT_PassiveAblity,      //被动
    EAT_GeneralAbility,     //普通
    EAT_ChannelAbility,     //引导
    EAT_ToggleAbility,      //开关
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
