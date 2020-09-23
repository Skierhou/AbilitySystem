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
        motionClip = new MotionClip(Vector3.one);
    }

    void Update()
    {
        CalculateVeloctiy();
        GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
        Debug.Log(GetComponent<CharacterController>().velocity);
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
        
    }
}
