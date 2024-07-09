using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    public string currentCone = "";
    public int cubeNumber = -1;
    private Rigidbody coneRigidbody;
    private float stabilityCheckDuration = 2.0f;
    private float stabilityCheckTimer = 0f;
    private bool coneStable = false;
    private bool coneInTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cone"))
        {
            coneRigidbody = other.GetComponent<Rigidbody>();
            stabilityCheckTimer = 0f;
            coneStable = false;
            coneInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cone"))
        {
            coneStable = false;
            coneInTrigger = false;
        }
    }

    void Update()
    {
        if (coneRigidbody != null && !coneStable)
        {
            if (coneRigidbody.velocity.magnitude < 0.01f && Mathf.Abs(coneRigidbody.angularVelocity.magnitude) < 0.01f)
            {
                stabilityCheckTimer += Time.deltaTime;
                if (stabilityCheckTimer >= stabilityCheckDuration && coneInTrigger)
                {
                    coneStable = true;
                    currentCone = coneRigidbody.gameObject.name;
                    Debug.Log(currentCone + " is stable on top of cube " + cubeNumber);
                }
            }
            else
            {
                stabilityCheckTimer = 0f;
            }
        }
    }
}
