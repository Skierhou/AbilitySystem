using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AbilityManager:Singleton<AbilityManager>
{
    Dictionary<FAbilityTagContainer, FAbilityBuffData> m_BuffDataMaps;
    Dictionary<FAbilityTagContainer, FAbilityData> m_AbilityDataMaps;

    public override void Initialize()
    {
        m_BuffDataMaps = new Dictionary<FAbilityTagContainer, FAbilityBuffData>();
        m_AbilityDataMaps = new Dictionary<FAbilityTagContainer, FAbilityData>();

    }

    public bool GetBuffData(FAbilityTagContainer inTag, out FAbilityBuffData outBuffData)
    {
        if (m_BuffDataMaps != null && m_BuffDataMaps.TryGetValue(inTag, out FAbilityBuffData buffData))
        {
            outBuffData = buffData;
            return true;
        }
        outBuffData = new FAbilityBuffData();
        return false;
    }

    public bool GetAbilityData(FAbilityTagContainer inTag, out FAbilityData outData)
    {
        if (m_AbilityDataMaps != null && m_AbilityDataMaps.TryGetValue(inTag, out FAbilityData data))
        {
            outData = data;
            return true;
        }
        outData = new FAbilityData();
        return false;
    }

    public AbilityBuff CreateAblityBuff(FAbilityTagContainer inTag)
    {
        return null;
    }
}