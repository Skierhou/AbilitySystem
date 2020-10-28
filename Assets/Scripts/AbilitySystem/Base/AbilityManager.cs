using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AbilityManager:Singleton<AbilityManager>
{
    private List<AbilityEditorData> AbilityDatas;
    Dictionary<FAbilityTagContainer, AbilityEditorData> m_AbilityMaps;

    Dictionary<FAbilityTagContainer, UnityAction<object, object, object>> eventMaps;

    public override void Initialize()
    {
        if (AbilityConsts.Instance is object)
        {
            AbilityDatas = new List<AbilityEditorData>();
            foreach (string path in AbilityConsts.Instance.abilityPathList)
            {
                //Resources.Load
                AbilityEditorData[] temp = Resources.LoadAll<AbilityEditorData>(path);
                AbilityDatas.AddRange(temp);
            }
            m_AbilityMaps = new Dictionary<FAbilityTagContainer, AbilityEditorData>();
            foreach (AbilityEditorData data in AbilityDatas)
            {
                m_AbilityMaps.Add(AbilityTagManager.Instance.GetTagContainer(data.abilityTags[0]), data);
            }
        }
        eventMaps = new Dictionary<FAbilityTagContainer, UnityAction<object, object, object>>();
    }

    public AbilityBase CreateAbility(FAbilityTagContainer inTag,AbilitySystemComponent systemComponent)
    {
        if (m_AbilityMaps.TryGetValue(inTag, out AbilityEditorData abilityEditorData) && abilityEditorData.AbilityScript != null)
        {
            AbilityBase ability = systemComponent.TryGetAbility(inTag);
            if (ability == null)
            {
                ability = System.Activator.CreateInstance(abilityEditorData.AbilityScript.GetClass()) as AbilityBase;
            }
            ability.InitAbility(systemComponent, abilityEditorData);
            return ability;
        }
        return null;
    }
    public AbilityBase CreateAbility(Type inType, AbilitySystemComponent systemComponent)
    {
        AbilityBase ability = System.Activator.CreateInstance(inType) as AbilityBase;
        return ability;
    }

    public void DestroyAbility(AbilityBase inAbility)
    {
        inAbility.DestroyAbility();
    }

    public void TriggerEvent(FAbilityTagContainer tagContainer, object param1, object param2, object param3)
    {
        if (eventMaps.TryGetValue(tagContainer, out UnityAction<object, object, object> action))
        {
            action?.Invoke(param1, param2, param3);
        }
    }
    public void RegisterEvent(FAbilityTagContainer tagContainer, UnityAction<object, object, object> register)
    {
        if (eventMaps.ContainsKey(tagContainer))
            eventMaps[tagContainer] += register;
        else
            eventMaps.Add(tagContainer, register);
    }
}