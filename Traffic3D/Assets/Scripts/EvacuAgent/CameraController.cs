using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float movementTime;

    [SerializeField]
    private float normalSpeed;

    [SerializeField]
    private float fastSpeed;

    [SerializeField]
    private float rotationAmount;

    [SerializeField]
    private Transform cameraTranfrom;

    [SerializeField]
    private Vector2 panLimit;

    [SerializeField]
    private Vector2 zoomLimit;

    [SerializeField]
    private Vector3 zoomAmount;

    [SerializeField]
    private Vector3 newPosition;

    [SerializeField]
    private Quaternion newRotation;

    [SerializeField]
    private Vector3 newZoom;

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTranfrom.localPosition;
    }

    void Update()
    {
        HandleCameraPan();
        HandleCameraRotation();
        HandleCameraZoom();
    }

    public void HandleCameraPan()
    {
        Vector3 temporaryPosition = transform.position;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        // Forward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            temporaryPosition += (transform.forward * movementSpeed);
        }
        // Backwards
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            temporaryPosition += (transform.forward * -movementSpeed);
        }
        // Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            temporaryPosition += (transform.right * movementSpeed);
        }
        // Left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            temporaryPosition += (transform.right * -movementSpeed);
        }

        // Limits the camera to a given position in the x and z axis (we use a vector2 where the y position is actuall the z for our panning)
        temporaryPosition.x = Mathf.Clamp(temporaryPosition.x, -panLimit.x, panLimit.x);
        temporaryPosition.z = Mathf.Clamp(temporaryPosition.z, -panLimit.y, panLimit.y);

        // For smoother movement
        transform.position = Vector3.Lerp(transform.position, temporaryPosition, Time.deltaTime * movementTime);
    }

    public void HandleCameraRotation()
    {
        // Rotate left
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        // Rotate right
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        // For smoother rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    public void HandleCameraZoom()
    {
        Vector3 temporaryZoom = cameraTranfrom.localPosition;

        // Zoom in
        if (Input.GetKey(KeyCode.Z))
        {
            temporaryZoom += zoomAmount;
        }
        // Zoom out
        if (Input.GetKey(KeyCode.C))
        {
            temporaryZoom -= zoomAmount;
        }

        // Limits the camera to a given level of zoom in the z axis. We use a Vector2 where the x maps to the minimum y and y maps to the maximum y.
        temporaryZoom.y = Mathf.Clamp(temporaryZoom.y, zoomLimit.x, zoomLimit.y);

        // For smoother zooming
        cameraTranfrom.localPosition = Vector3.Lerp(cameraTranfrom.localPosition, temporaryZoom, Time.deltaTime * movementTime);
    }
}
