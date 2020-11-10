using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class AttributeSet : IAttributeSet
{
    private Dictionary<EAttributeType, FAttributeData> attributeMaps = new Dictionary<EAttributeType, FAttributeData>();
    public Dictionary<EAttributeType, FAttributeData> AttributeMap { get { return attributeMaps; } }

    public void AddAttribute(EAttributeType attributeType, float baseValue, bool isNormalData = false)
    {
        if (!attributeMaps.ContainsKey(attributeType))
            attributeMaps.Add(attributeType, new FAttributeData(baseValue, isNormalData));
    }
    public void RegisterDataChangedEvent(EAttributeType attributeType, UnityAction<float, float> dataChanged)
    {
        if (attributeMaps.TryGetValue(attributeType, out FAttributeData data))
        {
            dataChanged?.Invoke(data.BaseValue, data.CurrentValue);
            data.OnDataChangedDeletage += dataChanged;
        }
    }
    public void RemoveDataChangedEvent(EAttributeType attributeType, UnityAction<float, float> dataChanged)
    {
        if (attributeMaps.TryGetValue(attributeType, out FAttributeData data))
        {
            data.OnDataChangedDeletage -= dataChanged;
        }
    }
    public bool GetAttributeData(EAttributeType attributeType,out FAttributeData attributeData)
    {
        if (attributeMaps.TryGetValue(attributeType, out FAttributeData data))
        {
            attributeData = data;
            return true;
        }
        attributeData = null;
        //attributeData = new FAttributeData();
        return false;
    }
    public FAttributeData GetAttributeData(EAttributeType attributeType)
    {
        attributeMaps.TryGetValue(attributeType,out FAttributeData data);
        return data;
    }

    public virtual void TakeDamage(int Damage)
    {
        if (attributeMaps.TryGetValue(EAttributeType.AT_Health, out FAttributeData attributeData))
        {
            attributeData.ChangeValue(extraDetlaValue: -Damage);
        }
    }
}