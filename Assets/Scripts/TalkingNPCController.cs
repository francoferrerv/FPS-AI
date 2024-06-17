using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TalkingNPCController: NPCController
{
    public override void TalkTo(ITalkingCharacter talkingCharacter)
    {
        Debug.Log($"{name}: I can talk to {talkingCharacter.name}.");
    }

    public override void StopTalkingTo(ITalkingCharacter talkingCharacter)
    {
        Debug.Log($"{name}: nobody wants to talk to me.");
    }
}
