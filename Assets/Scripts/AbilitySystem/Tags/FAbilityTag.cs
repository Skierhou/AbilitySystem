using System;
using System.Collections.Generic;
using UnityEngine;

public struct FAbilityTag
{
    private string m_TagName;
    private uint m_TagId;

    public string TagName { get => m_TagName; private set => m_TagName = value; }
    public uint TagId { get => m_TagId; private set => m_TagId = value; }

    public FAbilityTag(string inTagName)
    {
        if (string.IsNullOrEmpty(inTagName))
            Debug.LogError("AbilityTag build error! TagName is null");

        m_TagName = inTagName;
        m_TagId = CRC32.GetCRC32(m_TagName);
    }

    public FAbilityTag(string inTagName,FAbilityTag parent)
    {
        if (string.IsNullOrEmpty(inTagName))
            Debug.LogError("AbilityTag build error! TagName is null");

        m_TagName = parent.TagName + "." + inTagName;
        m_TagId = CRC32.GetCRC32(m_TagName);
    }
    public static bool operator ==(FAbilityTag abilityTag1, FAbilityTag abilityTag2)
    {
        return abilityTag1.TagId == abilityTag2.TagId;
    }
    public static bool operator !=(FAbilityTag abilityTag1, FAbilityTag abilityTag2)
    {
        return abilityTag1.TagId != abilityTag2.TagId || !abilityTag1.TagName.Equals(abilityTag2.TagName);
    }
    public override bool Equals(object obj)
    {
        if (obj is FAbilityTag data)
        {
            return TagId == data.TagId;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return TagName;
    }
}