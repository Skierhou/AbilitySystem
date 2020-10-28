using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 改变位移等修改器
/// </summary>
public class AbilityBuffMotionModifiers : AbilityBuffModifiers
{
    /* 优先级 */
    public int priority;
    public List<float> duration;

    public EMotionType moveType;
    public EDirectType direction;
    public List<float> distance;

    public EMotionType rotateType;
    public EDirectType rotateAxis;
    public List<float> rotateAngle;

    public AnimationCurve moveCurve;
    public AnimationCurve rotateCurve;

    public AbilityBuffMotionModifiers(AbilitySystemComponent abilitySystem, Editor_FMotionModifierData data)
    {
        this.abilitySystem = abilitySystem;

        priority = data.priority;
        duration = data.duration;
        moveType = data.moveType;
        distance = data.distance;
        rotateType = data.rotateType;
        rotateAxis = data.rotateAxis;
        rotateAngle = data.rotateAngle;
        moveCurve = AnimationCurveManager.Instance.GetCurve(data.moveCurve);
        rotateCurve = AnimationCurveManager.Instance.GetCurve(data.rotateCurve);
    }

    public override bool CanApplyModifier(int level = 0)
    {
        return abilitySystem.MovmentComponent.CanApplyMotion(priority);
    }
    public override void ApplyModifier(int level = 0)
    {
        abilitySystem.MovmentComponent.StartMotion();
        abilitySystem.MovmentComponent.ApplyMotion(priority, moveType, direction, distance[level], duration[level], moveCurve);
        if(rotateCurve != null)
            abilitySystem.MovmentComponent.ApplyRotate(priority, rotateType, abilitySystem.transform.rotation, rotateAxis, rotateAngle[level], duration[level], rotateCurve);
        else
            abilitySystem.MovmentComponent.ApplyRotate(priority, rotateType, rotateAxis, rotateAngle[level], duration[level]);
    }
}