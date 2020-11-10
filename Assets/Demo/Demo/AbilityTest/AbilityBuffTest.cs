using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBuffTest : AbilityBuff
{
    [AbilityConfig]
    public GameObject m_ParticleSystem;

    GameObject m_PSCInstance;

    protected override void SetActive(bool inActive)
    {
        base.SetActive(inActive);
        if (m_PSCInstance == null && m_ParticleSystem != null)
        {
            m_PSCInstance = GameObject.Instantiate(m_ParticleSystem, abilitySystem.transform);
            m_PSCInstance.transform.localScale = Vector3.one;
            m_PSCInstance.transform.localPosition = Vector3.up;
        }
        if(m_PSCInstance != null)
            m_PSCInstance.SetActive(inActive);
    } 
}
