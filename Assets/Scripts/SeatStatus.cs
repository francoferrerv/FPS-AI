using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeatStatus: MonoBehaviour
{
    public virtual bool GetAvailablePosition(out Vector3 position, out Vector3 eulerAngles, out string name)
    {
        eulerAngles = Vector3.zero;
        name = "";
        position = Vector3.zero;

        return false;
    }

    public virtual void mark(string name, bool available)
    {

    }
}
