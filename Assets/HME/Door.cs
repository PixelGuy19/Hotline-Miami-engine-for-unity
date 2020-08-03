using UnityEngine;

public class Door : MonoBehaviour
{
    void Start()
    {
        HingeJoint2D DoorJ = gameObject.GetComponent<HingeJoint2D>();
        float Min = 0;
        float Max = 0;
        if (transform.eulerAngles.z != 90)
        {
            Min = -120;
            Max = 120;
        }
        else
        {
            Min = -210;
            Max = 30;
        }
        JointAngleLimits2D Limits = new JointAngleLimits2D();
        Limits.max = Max;
        Limits.min = Min;
        DoorJ.limits = Limits;
    }
}