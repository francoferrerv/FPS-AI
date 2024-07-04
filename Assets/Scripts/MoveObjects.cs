using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float grabDistance = 5.0f; // Max distance to grab objects
    public float holdDistance = 5.0f; // Distance at which the object is held
    public float moveSpeed = 10.0f; // Speed at which the object moves
    public Transform playerTransform;

    private Camera mainCamera;
    private GameObject heldObject;
    private Rigidbody heldObjectRigidbody;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                DropObject();
            }
        }

        if (heldObject != null)
        {
            MoveObject();
        }
    }

    void TryPickUpObject()
    {
        Collider[] colliders = Physics.OverlapSphere(playerTransform.position, grabDistance);
        float closestDistance = grabDistance;
        Collider closestCollider = null;

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null)
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
        }

        if (closestCollider != null)
        {
            heldObject = closestCollider.gameObject;
            heldObjectRigidbody = heldObject.GetComponent<Rigidbody>();

            heldObjectRigidbody.useGravity = false;
            heldObjectRigidbody.drag = 10;
        }
    }

    void DropObject()
    {
        if (heldObjectRigidbody != null)
        {
            heldObjectRigidbody.useGravity = true;
            heldObjectRigidbody.drag = 1;
        }

        heldObject = null;
        heldObjectRigidbody = null;
    }

    void MoveObject()
    {
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * holdDistance;
        Vector3 direction = targetPosition - heldObject.transform.position;

        heldObjectRigidbody.velocity = direction * moveSpeed;
    }
}
