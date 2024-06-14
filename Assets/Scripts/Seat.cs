using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Seat
{
    public static GameObject getClosestSeat(Vector3 targetPosition)
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("Bench");
        GameObject closestSeat = null;
        float minimunDistance = float.PositiveInfinity;

        foreach (GameObject seat in seats)
        {
            Vector3 seatPosition = seat.transform.position;
            float distance = (targetPosition - seatPosition).magnitude;

            if (distance < minimunDistance)
            {
                minimunDistance = distance;
                closestSeat = seat;
            }
        }

        return closestSeat;
    }

    public static GameObject getRandomSeat()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Bench");

        if (chairs.Length > 0) {
            int chairIndex = Random.Range(0, chairs.Length);

            return chairs[chairIndex];
        }

        Debug.LogError("There are no chairs in the environment");

        return null;
    }
}
