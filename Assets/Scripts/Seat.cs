using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Seat
{
    public string availableName { get; private set; }
    public SeatStatus seatStatus { get; private set; }
    public Vector3 position { get; private set; }
    public Vector3 eulerAngles { get; private set; }

    public Seat(SeatStatus status, string name, Vector3 closestPosition)
    {
        availableName = name;
        seatStatus = status;
        position = closestPosition;
        eulerAngles = status.GetEulerAngles();
    }

    public static Seat getClosestSeat(Vector3 targetPosition)
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("Bench");
        BenchStatus closestBenchStatus = null;
        string closestName = "";
        Vector3 closestPosition = Vector3.zero;
        float minimunDistance = float.PositiveInfinity;

        foreach (GameObject seat in seats)
        {
            BenchStatus benchStatus = seat.GetComponent<BenchStatus>();
            Vector3 seatPosition;
            string name;

            if (benchStatus != null && benchStatus.GetAvailable(out name, out seatPosition))
            {
                float distance = (targetPosition - seatPosition).magnitude;

                if (distance < minimunDistance)
                {
                    minimunDistance = distance;
                    closestBenchStatus = benchStatus;
                    closestName = name;
                    closestPosition = seatPosition;
                }
            }
        }

        if (closestBenchStatus != null)
        {
            closestBenchStatus.mark(closestName, false);

            return new Seat(closestBenchStatus, closestName, closestPosition);
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
