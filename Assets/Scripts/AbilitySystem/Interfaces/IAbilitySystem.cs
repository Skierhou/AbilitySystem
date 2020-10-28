using System.Collections.Generic;
using UnityEngine.Events;

public interface IAbilitySystem
{
    // ----------------------------------------------------------------------------------------------------------------
    //	Ability
    // ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 尝试获取已有能力
    /// </summary>
    AbilityBase TryGetAbility(FAbilityTagContainer inTag);
    /// <summary>
    /// 获得能力
    /// </summary>
    void AcquireAbilityByTag(string inTag);
    /// <summary>
    /// 获得能力
    /// </summary>
    void AcquireAbilityByTag(FAbilityTagContainer inTag);
    /// <summary>
    /// 移除能力
    /// </summary>
    void RemoveAbilityByTag(string inTag);
    /// <summary>
    /// 移除能力
    /// </summary>
    void RemoveAbilityByTag(FAbilityTagContainer inTag);
    /// <summary>
    /// 获得能力
    /// </summary>
    void AcquireAbility(AbilityBase inAbility);
    /// <summary>
    /// 移除能力
    /// </summary>
    void RemoveAbility(AbilityBase inAbility);
    /// <summary>
    /// 尝试触发能力
    /// </summary>
    bool TryActivateAbilityByTag(string inTag);
    /// <summary>
    /// 尝试触发能力
    /// </summary>
    bool TryActivateAbilityByTag(FAbilityTagContainer inTag);
    /// <summary>
    /// 触发能力
    /// </summary>
    bool TryActivateAbility(AbilityBase inAbility);
    /// <summary>
    /// 是否可以激活能力
    /// </summary>
    bool CanActivateAbility(AbilityBase inAbility);
    /// <summary>
    /// 尝试打断能力
    /// </summary>
    bool TryCancelAbilityByTag(FAbilityTagContainer inTagContainer);
    /// <summary>
    /// 能力结束回调
    /// </summary>
    void OnEndAbility(AbilityBase inAbility);

    /// <summary>
    /// 注册事件
    /// </summary>
    void RegisterEvent(string inTag, UnityAction<AbilitySystemComponent> inAction);
    /// <summary>
    /// 注册事件
    /// </summary>
    void RegisterEvent(FAbilityTagContainer inTag, UnityAction<AbilitySystemComponent> inAction);
    /// <summary>
    /// 触发事件
    /// </summary>
    void TriggerEventByAbility(AbilityBase inAbility);

    // ----------------------------------------------------------------------------------------------------------------
    //	Buff
    // ----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 尝试激活Buff
    /// </summary>
    bool TryActivateBuffByTag(string inTag, int inLevel = 1, int inStackDelta = 1, bool bForceActivate = false);
    /// <summary>
    /// 尝试激活Buff
    /// </summary>
    bool TryActivateBuffByTag(FAbilityTagContainer inTag, int inLevel = 1, int inStackDelta = 1, bool bForceActivate = false);
    /// <summary>
    /// 尝试激活Buff
    /// </summary>
    bool TryActivateBuff(AbilityBuff buff, int inLevel = 1, int inStackDelta = 1, bool bForceActivate = false);

    // ----------------------------------------------------------------------------------------------------------------
    //	Other Check
    // ----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 检测当前作用源是否符合条件
    /// </summary>
    bool CheckSourceTags(AbilityBase inAbility);
    /// <summary>
    /// 检测当前作用目标是否符合条件
    /// </summary>
    bool CheckTargetTags(AbilityBase inAbility);
    /// <summary>
    /// 添加阻挡标签
    /// </summary>
    void AddBlockTags(List<FAbilityTagContainer> inTags);
    /// <summary>
    /// 移除阻挡标签
    /// </summary>
    void RemoveBlockTags(List<FAbilityTagContainer> inTags);
    /// <summary>
    /// 添加已激活标签
    /// </summary>
    void AddActivateTags(FAbilityTagContainer inTag);
    /// <summary>
    /// 添加已激活标签
    /// </summary>
    void AddActivateTags(List<FAbilityTagContainer> inTags);
    /// <summary>
    /// 移除已激活标签
    /// </summary>
    void RemoveActivateTags(FAbilityTagContainer inTag);
    /// <summary>
    /// 移除已激活标签
    /// </summary>
    void RemoveActivateTags(List<FAbilityTagContainer> inTags);
};