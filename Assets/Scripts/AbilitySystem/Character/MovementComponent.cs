using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ELifeType
{
    LT_Start,
    LT_Loop,
    LT_End,
};

public class MovementComponent : MonoBehaviour
{
    public Controller Controller { get; protected set; }
    public CharacterController CharacterController { get; protected set; }

    public bool IsDirectVelocity = true;
    public bool IsGrounded { get { return CharacterController.isGrounded; } }

    //Motion
    public MotionClip MotionClip { get; protected set; }
    public RootMotionClip RootMotionClip { get; protected set; }

    public Vector3 Gravity { get; protected set; }
    public Vector3 Velocity { get; protected set; }
    public Vector3 AngularVelocity { get; protected set; }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        MotionClip = new MotionClip(transform);
        RootMotionClip = new RootMotionClip(GetComponentInChildren<Animator>());
    }

    public virtual void OnPossess(Controller controller)
    {
        Controller = controller;
    }
    public virtual void OnUnPossess()
    {
        Controller = null;
    }

    protected void FixedUpdate()
    {
        if (MotionClip is object)
            MotionClip.TickMotion(Time.fixedDeltaTime);

        Velocity = CalculateVeloctiy();
        AngularVelocity = CalculateAngularVelocity();

        CharacterController.Move((Velocity + Gravity) * Time.fixedDeltaTime);
        Debug.Log(CharacterController.isGrounded);
        transform.eulerAngles += AngularVelocity * Time.fixedDeltaTime;
    }
    #region Motion
    public virtual Vector3 CalculateVeloctiy()
    {
        if (MotionClip != null && MotionClip.motionType == EMotionType.MT_Override)
        {
            return MotionClip.GetVelocity();
        }
        if (RootMotionClip != null && RootMotionClip.motionType == EMotionType.MT_Override)
        {
            return RootMotionClip.GetVelocity();
        }
        // 计算重力
        if (CharacterController.isGrounded)
            Gravity = Physics.gravity;
        else
            Gravity += Physics.gravity * Time.fixedDeltaTime;
        Vector3 res = Vector3.zero;
        if (MotionClip is object)
            res += MotionClip.GetVelocity();
        if (RootMotionClip is object)
            res += RootMotionClip.GetVelocity();
        if (Controller is object)
        {
            Vector3 inputVelocity = Controller.GetInputVelocity();
            res += inputVelocity;
            if (IsDirectVelocity && inputVelocity.sqrMagnitude >= 0.1f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputVelocity, Vector3.up), Time.fixedDeltaTime * 10);
        }
        return res;
    }

    public virtual Vector3 CalculateAngularVelocity()
    {
        if (MotionClip != null && MotionClip.rotateType == EMotionType.MT_Override)
        {
            return MotionClip.GetAngularVelocity();
        }
        if (RootMotionClip != null && RootMotionClip.rotateType == EMotionType.MT_Override)
        {
            return RootMotionClip.GetAngularVelocity();
        }
        Vector3 res = Vector3.zero;
        if (MotionClip is object)
            res += MotionClip.GetAngularVelocity();
        if (RootMotionClip is object)
            res += RootMotionClip.GetAngularVelocity();
        if (Controller is object)
            res += Controller.GetInputAngularVelocity();
        return res;
    }

    public bool CanApplyMotion(int priority)
    {
        return MotionClip.priority <= priority;
    }

    public void RegisterMotionEvent(ELifeType lifeType, UnityAction OnMotionEvent)
    {
        switch (lifeType)
        {
            case ELifeType.LT_Start:
                MotionClip.OnMotionStart += OnMotionEvent;
                break;
            case ELifeType.LT_Loop:
                MotionClip.OnMotionUpdate += OnMotionEvent;
                break;
            case ELifeType.LT_End:
                MotionClip.OnMotionEnd += OnMotionEvent;
                break;
        }
    } 
    public void RemoveMotionEvent(ELifeType lifeType, UnityAction OnMotionEvent)
    {
        switch (lifeType)
        {
            case ELifeType.LT_Start:
                MotionClip.OnMotionStart -= OnMotionEvent;
                break;
            case ELifeType.LT_Loop:
                MotionClip.OnMotionUpdate -= OnMotionEvent;
                break;
            case ELifeType.LT_End:
                MotionClip.OnMotionEnd -= OnMotionEvent;
                break;
        }
    }
    public void ApplyMotion(int priority, EMotionType motionType, EDirectType directType, float distance, float duration = 0.0f, AnimationCurve moveCurve = null)
    {
        MotionClip.ApplyMotion(priority, motionType, directType, distance, duration, moveCurve);
    }
    public void ApplyRotate(int priority, EMotionType rotateType, EDirectType asixType, float rotateAngle, float duration = 0.0f, AnimationCurve rotateCurve = null)
    {
        MotionClip.ApplyRotate(priority, rotateType, asixType, rotateAngle, duration, rotateCurve);
    }
    #endregion
}