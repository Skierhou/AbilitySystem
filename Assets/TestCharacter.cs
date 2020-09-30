using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    Vector3 InputVelocity;
    Vector3 InputAccel;
    MotionClip motionClip;
    RootMotionClip rootMotionClip;

    Vector3 velocity;
    Vector3 rotateE;

    private void Start()
    {
        motionClip = new MotionClip(Vector3.zero);
        rootMotionClip = new RootMotionClip(null);
    }

    void Update()
    {
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,rotateE,Time.deltaTime * 10);
    }

    private void FixedUpdate()
    {
        //计算
        CalculateVeloctiy();
        CalculateRotate();
        //处理
        Debug.Log(GetComponent<CharacterController>().isGrounded);
        GetComponent<CharacterController>().Move((Vector3.up * -9.8f + velocity) * Time.deltaTime);
    }

    void CalculateVeloctiy()
    {
        if (motionClip != null && motionClip.motionType == EMotionType.MT_Override)
        {
            velocity = motionClip.GetVelocity();
            return;
        }
        if (rootMotionClip != null && rootMotionClip.motionType == EMotionType.MT_Override)
        {
            velocity = rootMotionClip.GetVelocity();
            return;
        }
        InputAccel.x = Input.GetAxis("Horizontal");
        InputAccel.z = Input.GetAxis("Vertical");
        velocity = motionClip.GetVelocity() + rootMotionClip.GetVelocity() + InputAccel * 10;
    }

    void CalculateRotate()
    {
        if (motionClip != null && motionClip.rotateType == EMotionType.MT_Override)
        {
            rotateE = motionClip.GetRotateRate();
            return;
        }
        if (rootMotionClip != null && rootMotionClip.rotateType == EMotionType.MT_Override)
        {
            rotateE = rootMotionClip.GetRotateRate();
            return;
        }
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        rotateE = motionClip.GetRotateRate() + rootMotionClip.GetRotateRate() + new Vector3(-y, x, 0) * 10 + transform.eulerAngles;
    }
}
