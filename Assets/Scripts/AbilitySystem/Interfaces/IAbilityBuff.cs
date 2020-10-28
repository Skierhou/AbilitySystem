using System.Collections.Generic;

public interface IAbilityBuff
{
    /// <summary>
    /// 初始化
    /// </summary>
    void InitBuffData(Editor_FAbilityBuffData inBuffData,List<Editor_FModifierData> inModifierData,List<Editor_FMotionModifierData> inMotionModifierData);
    /// <summary>
    /// 初始化
    /// </summary>
    void InitAbility(AbilitySystemComponent abilitySystem,AbilityEditorData abilityEditorData);
    /// <summary>
    /// 尝试激活
    /// </summary>
    bool TryActivateAbility(AbilitySystemComponent inTargetSystem = null);
    /// <summary>
    /// 结束
    /// </summary>
    void EndAbility();
    /// <summary>
    /// 是否可激活
    /// </summary>
    bool CanActivateAbility();
    /// <summary>
    /// 是否可以消耗
    /// </summary>
    bool CanApplyModifier();
    /// <summary>
    /// 打断能力
    /// </summary>
    void CancelAbility();
    /// <summary>
    /// 更新Buff
    /// </summary>
    void UpdateLevelAndStack(int inLevel = 1, int inStackDelta = 1);
}