using System;
using System.Collections.Generic;

public class AttributeSet
{
    private Dictionary<EAttributeType, FAttributeData> attributeMaps = new Dictionary<EAttributeType, FAttributeData>();


    public void AddAttribute(EAttributeType attributeType,float baseValue)
    {
        if (!attributeMaps.ContainsKey(attributeType))
            attributeMaps.Add(attributeType,new FAttributeData(baseValue));
    }

    public bool GetAttributeData(EAttributeType attributeType,out FAttributeData attributeData)
    {
        if (attributeMaps.TryGetValue(attributeType, out FAttributeData data))
        {
            attributeData = data;
            return true;
        }
        attributeData = new FAttributeData();
        return false;
    }
}