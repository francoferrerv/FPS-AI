using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BenchStatus: SeatStatus
{
    protected bool leftAvailable = true;
    protected bool rightAvailable = true;
    protected Vector3 leftPosition = Vector3.zero;
    protected Vector3 rightPosition = Vector3.zero;
    protected Vector3 eulerAngles = Vector3.zero;

    protected void Start()
    {
        leftPosition = transform.position
            + transform.forward * 1f
            - transform.right * 1f;
        rightPosition = transform.position
            + transform.forward * 1f
            + transform.right * 1f;
        eulerAngles = transform.rotation.eulerAngles;
    }
    public override bool IsAvailable()
    {
        return leftAvailable || rightAvailable;
    }


    public override bool GetAvailable(out string name, out Vector3 position)
    {
        name = leftAvailable ? "left" : "right";
        position = leftAvailable ? leftPosition : rightPosition;

        return IsAvailable();
    }

    public override Vector3 GetEulerAngles()
    {
        return eulerAngles;
    }

    public override void SetAvailability(string name, bool available)
    {
        if (name == "left")
        {
            leftAvailable = available;
        }
        else
        {
            rightAvailable = available;
        }
    }
}
