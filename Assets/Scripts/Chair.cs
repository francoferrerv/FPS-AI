using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Chair
{
    public static GameObject getRandomChair()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");

        if (chairs.Length > 0) {
            int chairIndex = Random.Range(0, chairs.Length);
            return chairs[chairIndex];
        }

        Debug.LogError("There are no chairs in the environment");
        return null;
    }
}
