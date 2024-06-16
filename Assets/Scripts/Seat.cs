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

    public static Seat getRandomSeat()
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("Bench");
        GameObject[] availableSeats = Array.FindAll(seats, seat =>
        {
            Vector3 seatPosition;
            string placeName;
            BenchStatus benchStatus = seat.GetComponent<BenchStatus>();

            return benchStatus.GetAvailable(out placeName, out seatPosition);
        });

        if (availableSeats.Length > 0) {
            int seatIndex = UnityEngine.Random.Range(0, availableSeats.Length);
            Vector3 seatPosition;
            string placeName;
            GameObject seat = availableSeats[seatIndex];
            BenchStatus benchStatus = seat.GetComponent<BenchStatus>();

            benchStatus.GetAvailable(out placeName, out seatPosition);
            benchStatus.SetAvailability(placeName, false);

            return new Seat(benchStatus, placeName, seatPosition);
        }

        return null;
    }
}
