using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPhysicsType
{
    PT_None,
    PT_Falling,
    PT_Grounded,
    PT_Flying,
    PT_Swiming,
    PT_Climb,
};

class Character: MonoBehaviour
{
    #region Properity
    //Normal
    public EPhysicsType physicsType;

    protected Controller controller;

    //Motion
    private MotionClip motionClip;
    private RootMotionClip rootMotionClip;
    protected MotionClip MotionClip { get => motionClip; set => motionClip = value; }
    protected RootMotionClip RootMotionClip { get => rootMotionClip; set => rootMotionClip = value; }

    protected Vector3 m_velocity;
    protected Vector3 m_angularVelocity;
    #endregion

    #region Unity Life
    protected void FixedUpdate()
    {
        if(motionClip is object)
            motionClip.TickMotion();
        CalculateVeloctiy();

        transform.position += m_velocity * Time.fixedDeltaTime;
    }
    #endregion

    #region Motion
    void CalculateVeloctiy()
    {
        if (MotionClip != null && MotionClip.motionType == EMotionType.MT_Override)
        {
            m_velocity = MotionClip.GetVelocity();
            return;
        }
        if (RootMotionClip != null && RootMotionClip.motionType == EMotionType.MT_Override)
        {
            m_velocity = RootMotionClip.GetVelocity();
            return;
        }
        m_velocity = MotionClip.GetVelocity() + RootMotionClip.GetVelocity() + GetInputVelocity();
    }
    //void CalculateRotate()
    //{
    //    if (MotionClip != null && MotionClip.rotateType == EMotionType.MT_Override)
    //    {
    //        m_angularVelocity = MotionClip.GetRotateRate();
    //        return;
    //    }
    //    if (RootMotionClip != null && RootMotionClip.rotateType == EMotionType.MT_Override)
    //    {
    //        m_angularVelocity = RootMotionClip.GetRotateRate();
    //        return;
    //    }
    //    m_angularVelocity = MotionClip.GetRotateRate() + RootMotionClip.GetRotateRate() + GetInputAngularVelocity();
    //}
    protected virtual Vector3 GetInputVelocity()
    {
        return Vector3.zero;
        //return playerInput == null ? Vector3.zero : playerInput.GetInputVelocity();
    }
    protected virtual Vector3 GetInputAngularVelocity()
    {
        return Vector3.zero;
        //return playerInput == null ? Vector3.zero : playerInput.GetInputAngularVelocity();
    }
    #endregion

}