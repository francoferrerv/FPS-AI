using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeatStatus: MonoBehaviour
{
    public virtual bool GetAvailable(out string name, out Vector3 position)
    {
        name = "";
        position = Vector3.zero;

        return false;
    }

    public virtual Vector3 GetEulerAngles()
    {
        return Vector3.zero;
    }

    public virtual void SetAvailability(string name, bool available)
    {

    }
}
