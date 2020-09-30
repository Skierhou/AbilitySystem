using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class RootMotionClip
{
    public EMotionType motionType;
    public EMotionType rotateType;

    Animator animator;

    public RootMotionClip(Animator animator)
    {
        this.animator = animator;
    }
    public virtual Vector3 GetVelocity()
    {
        return animator == null ? Vector3.zero : animator.velocity;
    }
    public virtual Vector3 GetRotateRate()
    {
        return animator == null ? Vector3.zero : animator.angularVelocity;
    }
}