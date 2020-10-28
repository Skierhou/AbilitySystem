using System;
using System.Collections.Generic;

public struct FAbilityTagContainer
{
    /* Tag List example: [0]:GameAbility/[1]:Burn/[2]:Damage */
    public List<FAbilityTag> abilityTags;

    public FAbilityTagContainer(params FAbilityTag[] abilityTags)
    {
        this.abilityTags = null;
        for (int i = 0; i < abilityTags.Length; i++)
        {
            AddAbilityTag(abilityTags[i]);
        }
    }

    public void AddAbilityTag(FAbilityTag inAbilityTag)
    {
        if (abilityTags == null)
            abilityTags = new List<FAbilityTag>();

        abilityTags.Add(inAbilityTag);
    }

    public bool HasTag(FAbilityTag inAbilityTag)
    {
        if (abilityTags is null) return false;

        return abilityTags.Contains(inAbilityTag);
    }

    public bool IsEmpty() { return abilityTags is null ? true : abilityTags.Count == 0; }

    public bool HasAll(FAbilityTagContainer tagContainer)
    {
        if (tagContainer.IsEmpty()) return true;
        if (IsEmpty() || abilityTags.Count < tagContainer.abilityTags.Count) return false;

        for (int i = 0; i < tagContainer.abilityTags.Count; i++)
        {
            if (abilityTags[i] != tagContainer.abilityTags[i])
                return false;
        }
        return true;
    }
    public static bool operator ==(FAbilityTagContainer abilityTag1, FAbilityTagContainer abilityTag2)
    {
        if (abilityTag1.IsEmpty() || abilityTag2.IsEmpty()) return false;
        if (abilityTag1.abilityTags.Count != abilityTag2.abilityTags.Count) return false;

        for (int i = 0; i < abilityTag1.abilityTags.Count; i++)
        {
            if (abilityTag1.abilityTags[i] != abilityTag2.abilityTags[i]) return false;
        }
        return true;
    }
    public static bool operator !=(FAbilityTagContainer abilityTag1, FAbilityTagContainer abilityTag2)
    {
        if (abilityTag1.IsEmpty() || abilityTag2.IsEmpty()) return true;
        if (abilityTag1.abilityTags.Count != abilityTag2.abilityTags.Count) return true;

        for (int i = 0; i < abilityTag1.abilityTags.Count; i++)
        {
            if (abilityTag1.abilityTags[i] != abilityTag2.abilityTags[i]) return true;
        }
        return false;
    }
    public override bool Equals(object obj)
    {
        if (obj is FAbilityTagContainer data)
        {
            if (IsEmpty() || data.IsEmpty()) return false;
            if (abilityTags.Count != data.abilityTags.Count) return false;

            for (int i = 0; i < abilityTags.Count; i++)
            {
                if (abilityTags[i] != data.abilityTags[i]) return false;
            }
            return true;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string str = "";
        if (abilityTags != null && abilityTags.Count > 0)
        {
            //for (int i = 0; i < abilityTags.Count - 1; i++)
            //{
            //    str += abilityTags[i].TagName + "|";
            //}
            str = abilityTags[abilityTags.Count - 1].TagName;
        }
        return str;
    }
}