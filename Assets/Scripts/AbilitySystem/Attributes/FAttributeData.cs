using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FAttributeData
{
    private float baseValue;
    private float extraValue;
    private float extraValue_AcceptMul;
    private float multiScale;
    private bool isNormalData;      //是否为血量，蓝量等基础数值
    public UnityAction<float, float> OnDataChangedDeletage;

    //如血量，蓝量等，BaseValue=Max，CurrentValue=Current，修改ExtraValue 来修改Current
    //如攻击力，防御等，BaseValue=Base(白字),CurrentValue=Total(总共),ExtraValue 以及 ExtraValue_AcceptMul=(绿字:装备加成)
    public float BaseValue { get => baseValue * (1 + multiScale);  }
    public float CurrentValue { get => BaseValue + ExtraValue + ExtraValue_AcceptMul; }
    public float ExtraValue { get => extraValue;  }
    public float ExtraValue_AcceptMul { get => extraValue_AcceptMul * (1 + multiScale);  }
    public float MultiScale { get => multiScale; }

    public FAttributeData(float inValue,bool inIsNormalData = true)
    {
        baseValue = inValue;
        isNormalData = inIsNormalData;
        extraValue = 0;
        extraValue_AcceptMul = 0;
        multiScale = 0;
        OnDataChangedDeletage = null;
    }

    public void ChangeValue(float baseDetlaValue = 0, float extraDetlaValue = 0, float extraDeltaValue_AcceptMul = 0, float mulitDeltaScale = 0)
    {
        baseValue += baseDetlaValue;
        extraValue += extraDetlaValue;
        extraValue_AcceptMul += extraDeltaValue_AcceptMul;
        multiScale += mulitDeltaScale;

        baseValue = Mathf.Clamp(baseValue, 0.0f, float.MaxValue);
        if (isNormalData)
            extraValue = Mathf.Clamp(extraValue, -BaseValue, 0.0f);

        OnDataChangedDeletage?.Invoke(BaseValue, CurrentValue);
    }
}