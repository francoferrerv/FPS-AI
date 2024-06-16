using UnityEngine;
using UnityEngine.UI;
using System;
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
        List<BenchStatus> benchesStatus = getAvailableBenchesStatuses();

        if (benchesStatus.Count <= 0)
        {
            return null;
        }

        string closestPlaceName = "";
        float minimunDistance = float.PositiveInfinity;
        BenchStatus closestBenchStatus = null;
        Vector3 closestPosition = Vector3.zero;

        foreach (var benchStatus in benchesStatus)
        {
            Vector3 seatPosition;
            string placeName;

            if (benchStatus.GetAvailable(out placeName, out seatPosition))
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

        closestBenchStatus.SetAvailability(closestPlaceName, false);

        return new Seat(closestBenchStatus, closestPlaceName, closestPosition);
    }

    public static Seat getRandomSeat()
    {
        List<BenchStatus> benchesStatus = getAvailableBenchesStatuses();

        if (benchesStatus.Count > 0)
        {
            Vector3 seatPosition;
            string placeName;
            int seatIndex = UnityEngine.Random.Range(0, benchesStatus.Count);
            BenchStatus benchStatus = benchesStatus[seatIndex];

            benchStatus.GetAvailable(out placeName, out seatPosition);
            benchStatus.SetAvailability(placeName, false);

            return new Seat(benchStatus, placeName, seatPosition);
        }

        return null;
    }


    private static List<BenchStatus> getAvailableBenchesStatuses()
    {
        List<BenchStatus> benchesStatus = new List<BenchStatus>();
        GameObject[] benches = GameObject.FindGameObjectsWithTag("Bench");

        foreach (var bench in benches)
        {
            Vector3 benchPosition;
            string placeName;
            BenchStatus benchStatus = bench.GetComponent<BenchStatus>();

            if (benchStatus.GetAvailable(out placeName, out benchPosition))
            {
                benchesStatus.Add(benchStatus);
            }
        }

        return benchesStatus;
    }
}
