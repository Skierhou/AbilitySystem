using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAttributeDataChanged(float baseValue,float currentValue);

public struct FAttributeData
{
    private float baseValue;
    private float extraValue;
    private float extraValue_AcceptMul;
    private float multiScale;
    public event OnAttributeDataChanged OnDataChangedDeletage;

    public float BaseValue { get => baseValue * (1 + multiScale);  }
    public float CurrentValue { get => BaseValue + ExtraValue + ExtraValue_AcceptMul; }
    public float ExtraValue { get => extraValue;  }
    public float ExtraValue_AcceptMul { get => extraValue_AcceptMul * (1 + multiScale);  }
    public float MultiScale { get => multiScale; }

    public FAttributeData(float inValue)
    {
        baseValue = inValue;
        extraValue = 0;
        extraValue_AcceptMul = 0;
        multiScale = 0;
        OnDataChangedDeletage = null;
    }

    public void ChangeValue(float baseDetlaValue, float extraDetlaValue, float extraDeltaValue_AcceptMul, float mulitDeltaScale)
    {
        baseValue += baseDetlaValue;
        extraValue += extraDetlaValue;
        extraValue_AcceptMul += extraDeltaValue_AcceptMul;
        multiScale += mulitDeltaScale;

        baseValue = Mathf.Clamp(baseValue, 0.0f, float.MaxValue);

        OnDataChangedDeletage?.Invoke(BaseValue, CurrentValue);
    }

    public void ClearEventListener()
    {
        if (OnDataChangedDeletage == null) return;
        Delegate[] delegates = OnDataChangedDeletage.GetInvocationList();
        foreach (Delegate del in delegates)
        {
            OnDataChangedDeletage -= del as OnAttributeDataChanged;
        }
    }
}