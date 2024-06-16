using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Seat
{
    public Vector3 position { get; private set; }
    public Vector3 eulerAngles { get; private set; }

    public Seat(Vector3 pos, Vector3 angles)
    {
        position = pos;
        eulerAngles = angles;
    }

    public static Seat getClosestSeat(Vector3 targetPosition)
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("Bench");
        BenchStatus closestBenchStatus = null;
        Vector3 closestPosition = Vector3.zero;
        Vector3 closestEulerAngles = Vector3.zero;
        string closestName = "";
        float minimunDistance = float.PositiveInfinity;

        foreach (GameObject seat in seats)
        {
            BenchStatus benchStatus = seat.GetComponent<BenchStatus>();
            Vector3 seatPosition, eulerAngles;
            string name;

            if (benchStatus && benchStatus.GetAvailablePosition(out seatPosition, out eulerAngles, out name))
            {
                float distance = (targetPosition - seatPosition).magnitude;

                if (distance < minimunDistance)
                {
                    minimunDistance = distance;
                    closestBenchStatus = benchStatus;
                    closestPosition = seatPosition;
                    closestEulerAngles = eulerAngles;
                    closestName = name;
                }
            }
        }

        if (closestBenchStatus != null)
        {
            closestBenchStatus.mark(closestName, false);

            return new Seat(closestPosition, closestEulerAngles);
        }

        return null;
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
