public interface IAbility
{
    /// <summary>
    /// 初始化
    /// </summary>
    void InitAbility(AbilitySystemComponent abilitySystem, AbilityEditorData abilityEditorData);
    /// <summary>
    /// 提交冷却
    /// </summary>
    void CommitCooldown();
    /// <summary>
    /// 提交消耗
    /// </summary>
    void CommitCost();
    /// <summary>
    /// 提交冷却以及消耗
    /// </summary>
    void CommitAbility();
    /// <summary>
    /// 打断当前技能
    /// </summary>
    void CancelAbility();
    /// <summary>
    /// 主动结束技能
    /// </summary>
    void EndAbility();
    /// <summary>
    /// 是否可激活
    /// </summary>
    bool CanActivateAbility();
    /// <summary>
    /// 尝试激活技能
    /// </summary>
    bool TryActivateAbility(AbilitySystemComponent inTargetAbilitySystem = null);
    /// <summary>
    /// 是否可以消耗
    /// </summary>
    bool CanCost();
}
