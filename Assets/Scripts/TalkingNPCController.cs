using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TalkingNPCController: BaseController
{
    protected override void Update()
    {
        base.Update();

        moveChairTest();
    }

    protected void moveChairTest()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state == NPCState.Idle)
            {
                state = sitOnClosestSeat();
            }
            else if (state == NPCState.Sitting)
            {
                state = standUp();
            }
        }
    }
}
