using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 改变位移等修改器
/// </summary>
public class AbilityBuffMotionModifiers : AbilityBuffModifiers
{
    CharacterController m_CC;
    Transform m_Target;

    public AbilityBuffMotionModifiers(AbilitySystemComponent systemComponent)
    {
        m_CC = systemComponent.GetComponent<CharacterController>();
    }

    public override bool CanApplyModifier(int level = 0)
    {
        return base.CanApplyModifier(level);
    }
    public override void ApplyModifier(int level = 0)
    {
        base.ApplyModifier(level);
    }
    void ApplyMotion()
    {
        
    }

    void MoveTo(Vector3 inWorldLoc, float inSpeed)
    {
        MovePoision((inWorldLoc - m_Target.position).normalized * inSpeed * Time.deltaTime);
    }
    void MovePoision(Vector3 inMovePos)
    {
        m_CC.Move(inMovePos);
    }
    void Rotate(Quaternion inRot, float inSpeed)
    {
        m_Target.rotation = Quaternion.LerpUnclamped(m_Target.rotation, inRot, inSpeed);
    }
    void Rotate(Vector3 inEuler, float inSpeed)
    {
        m_Target.rotation = Quaternion.LerpUnclamped(m_Target.rotation, Quaternion.LookRotation(inEuler), inSpeed);
    }
}