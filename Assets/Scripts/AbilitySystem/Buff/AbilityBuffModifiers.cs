using System.Collections.Generic;
using UnityEngine;

public class AbilityBuffModifiers
{
    public EAttributeType attributeType;
    public EBuffModifierOption modifierOption;
    public EBuffModifierType modifierType;
    public List<float> attributeMagnitudeList;
    public AbilitySystemComponent abilitySystem;

    public AbilityBuffModifiers() { }
    public AbilityBuffModifiers(AbilitySystemComponent abilitySystem, Editor_FModifierData inData)
    {
        this.abilitySystem = abilitySystem;
        attributeType = inData.attributeType;
        modifierOption = inData.modifierOption;
        modifierType = inData.modifierType;
        attributeMagnitudeList = inData.attributeMagnitudeList;
    }

    public virtual bool CanApplyModifier(int level = 0)
    {
        if (attributeMagnitudeList != null && attributeMagnitudeList.Count <= level)
            level = attributeMagnitudeList.Count - 1;

        if (abilitySystem.AttributeSet.GetAttributeData(attributeType, out FAttributeData data) && level >= 0)
        {
            switch (modifierOption)
            {
                case EBuffModifierOption.EBMO_Add:
                    return data.CurrentValue + attributeMagnitudeList[level] >= 0;
                case EBuffModifierOption.EBMO_Mul:
                    return data.CurrentValue * attributeMagnitudeList[level] >= 0;
                case EBuffModifierOption.EBMO_Divide:
                    return data.CurrentValue / attributeMagnitudeList[level] >= 0;
                case EBuffModifierOption.EBMO_Override:
                    return attributeMagnitudeList[level] >= 0;
            }
        }
        return false;
    }

    public virtual void ApplyModifier(int level = 0)
    {
        if (abilitySystem.AttributeSet.GetAttributeData(attributeType, out FAttributeData data)
            && attributeMagnitudeList != null
            && attributeMagnitudeList.Count > level)
        {
            float baseChange = 0, extraChange = 0, extraChange_AcceptMul = 0, mulitScaleChange = 0;

            switch (modifierOption)
            {
                case EBuffModifierOption.EBMO_Add:
                    baseChange = extraChange = attributeMagnitudeList[level];
                    break;
                case EBuffModifierOption.EBMO_Mul:
                    mulitScaleChange = attributeMagnitudeList[level] - 1;
                    break;
                case EBuffModifierOption.EBMO_Divide:
                    mulitScaleChange = 1.0f / attributeMagnitudeList[level] - 1;
                    break;
                case EBuffModifierOption.EBMO_Override:
                    baseChange = attributeMagnitudeList[level] - data.BaseValue;
                    extraChange = attributeMagnitudeList[level] - data.ExtraValue;
                    break;
            }
            switch (modifierType)
            {
                case EBuffModifierType.EBMT_Base:
                    extraChange = 0;
                    break;
                case EBuffModifierType.EBMT_Extra:
                    baseChange = 0;
                    break;
                case EBuffModifierType.EBMT_Extra_AcceptMul:
                    extraChange_AcceptMul = extraChange;
                    baseChange = extraChange = 0;
                    break;
            }
            data.ChangeValue(baseChange, extraChange, extraChange_AcceptMul, mulitScaleChange);
        }
    }
}