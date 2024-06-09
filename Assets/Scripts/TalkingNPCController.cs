using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class TalkingNPCController: BaseNPCController
{
    protected override void Update()
    {
        base.Update();

        if (!NetworkManager.normalGameStart)
        {
            moveChairTest();
        }
    }

    protected void moveChairTest()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state == NPCState.Idle)
            {
                state = sitOnRandomChair();
            }
            else if (state == NPCState.Sitting)
            {
                state = standUp();
            }
        }
    }
}
