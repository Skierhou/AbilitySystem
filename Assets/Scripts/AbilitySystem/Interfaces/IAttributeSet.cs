using UnityEngine.Events;

public interface IAttributeSet
{
    /// <summary>
    /// 添加一个属性
    /// </summary>
    void AddAttribute(EAttributeType attributeType, float baseValue, bool isNormalData = false);
    /// <summary>
    /// 注册数值变化事件
    /// </summary>
    void RegisterDataChangedEvent(EAttributeType attributeType, UnityAction<float, float> dataChanged);
    /// <summary>
    /// 移除数值变化事件
    /// </summary>
    void RemoveDataChangedEvent(EAttributeType attributeType, UnityAction<float, float> dataChanged);
    /// <summary>
    /// 获取数值
    /// </summary>
    bool GetAttributeData(EAttributeType attributeType, out FAttributeData attributeData);
    /// <summary>
    /// 获取数值
    /// </summary>
    FAttributeData GetAttributeData(EAttributeType attributeType);
    /// <summary>
    /// 造成伤害
    /// </summary>
    void TakeDamage(int Damage);
};