using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SeatStatus: MonoBehaviour
{
    public abstract bool IsAvailable();

    public abstract bool GetAvailable(out string name, out Vector3 position);

    public abstract Vector3 GetEulerAngles();

    public abstract void SetAvailability(string name, bool available);

    public abstract void SetTalkingCharacter(string name, ITalkingCharacter talkingCharacter);
}
