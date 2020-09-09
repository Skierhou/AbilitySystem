using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    Vector3 InputVelocity;
    Vector3 InputAccel;
    MotionClip motionClip;

    Vector3 velocity;

    void Update()
    {

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
        
    }
}
