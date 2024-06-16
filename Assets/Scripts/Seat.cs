using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Seat
{
    public string placeName { get; private set; }
    public SeatStatus status { get; private set; }
    public Vector3 position { get; private set; }
    public Vector3 eulerAngles { get; private set; }

    public Seat(SeatStatus status, string placeName, Vector3 position)
    {
        this.placeName = placeName;
        this.status = status;
        this.position = position;
        this.eulerAngles = status.GetEulerAngles();
    }

    public static Seat getClosestSeat(Vector3 targetPosition)
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("Bench");
        BenchStatus closestBenchStatus = null;
        string closestPlaceName = "";
        Vector3 closestPosition = Vector3.zero;
        float minimunDistance = float.PositiveInfinity;

        foreach (GameObject seat in seats)
        {
            BenchStatus benchStatus = seat.GetComponent<BenchStatus>();
            Vector3 seatPosition;
            string placeName;

            if (benchStatus != null && benchStatus.GetAvailable(out placeName, out seatPosition))
            {
                float distance = (targetPosition - seatPosition).magnitude;

                if (distance < minimunDistance)
                {
                    minimunDistance = distance;
                    closestBenchStatus = benchStatus;
                    closestPlaceName = placeName;
                    closestPosition = seatPosition;
                }
            }
        }

        if (closestBenchStatus != null)
        {
            closestBenchStatus.SetAvailability(closestPlaceName, false);

            return new Seat(closestBenchStatus, closestPlaceName, closestPosition);
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
