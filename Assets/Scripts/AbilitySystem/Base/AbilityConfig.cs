using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Struct, AllowMultiple = false)]
public class AbilityConfig : Attribute
{
    public AbilityConfig() { }
}