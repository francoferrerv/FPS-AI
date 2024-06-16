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
    protected Vector3 angles = Vector3.zero;

    protected void Start()
    {
        leftPosition = transform.position
            + transform.forward * 1.2f
            - transform.right * 1.5f;
        rightPosition = transform.position
            + transform.forward * 1.2f
            + transform.right * 1.5f;
        angles = transform.rotation.eulerAngles;
    }

    public override bool GetAvailablePosition(out Vector3 position, out Vector3 eulerAngles, out string name)
    {
        if (!leftAvailable && !rightAvailable)
        {
            eulerAngles = Vector3.zero;
            name = "";
            position = Vector3.zero;

            return false;
        }

        eulerAngles = angles;
        name = leftAvailable ? "left" : "right";
        position = leftAvailable ? leftPosition : rightPosition;

        return true;
    }

    public virtual void mark(string name, bool available)
    {
        if (name == "left")
        {
            leftAvailable = available;
        }
        else if (name == "right")
        {
            rightAvailable = available;
        }
    }
}
