using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TalkingPlayerController: PlayerController
{
    public override void TalkTo(ITalkingCharacter talkingCharacter)
    {
        base.TalkTo(talkingCharacter);
        Debug.Log($"{name}: I can talk to {talkingCharacter.name}.");
    }

    public override void StopTalkingTo(ITalkingCharacter talkingCharacter)
    {
        base.StopTalkingTo(talkingCharacter);
        Debug.Log($"{name}: nobody wants to talk to me.");
    }
}
