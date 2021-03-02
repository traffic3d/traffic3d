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
    private Quaternion newRotation;

    private const float buttonNotPressedValue = 0f;
    private const string horizonatalAxis = "Horizontal";
    private const string verticalAxis = "Vertical";
    private const string rotationAxis = "Fire1";
    private const string zoomAxis = "Fire2";
    private const string increasedPanSpeedAxis = "Fire3";

    void Start()
    {
        newRotation = transform.rotation;
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

        float horizontalPanVale = Input.GetAxis(horizonatalAxis);
        float verticalPanValue = Input.GetAxis(verticalAxis);
        float increasedPanSpeed = Input.GetAxis(increasedPanSpeedAxis);

        if (increasedPanSpeed > buttonNotPressedValue)
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        // Forward
        if (verticalPanValue > buttonNotPressedValue)
        {
            temporaryPosition += (transform.forward * movementSpeed);
        }
        // Backwards
        if (verticalPanValue < buttonNotPressedValue)
        {
            temporaryPosition += (transform.forward * -movementSpeed);
        }
        // Right
        if (horizontalPanVale > buttonNotPressedValue)
        {
            temporaryPosition += (transform.right * movementSpeed);
        }
        // Left
        if (horizontalPanVale < buttonNotPressedValue)
        {
            temporaryPosition += (transform.right * -movementSpeed);
        }

        // Limits the camera to a given position in the x and z axis (we use a vector2 where the y position is actually the z for our panning)
        temporaryPosition.x = Mathf.Clamp(temporaryPosition.x, -panLimit.x, panLimit.x);
        temporaryPosition.z = Mathf.Clamp(temporaryPosition.z, -panLimit.y, panLimit.y);

        // For smoother movement
        transform.position = Vector3.Lerp(transform.position, temporaryPosition, Time.deltaTime * movementTime);
    }

    public void HandleCameraRotation()
    {
        float rotationValue = Input.GetAxis(rotationAxis);

        // Rotate left
        if (rotationValue < buttonNotPressedValue)
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        // Rotate right
        if (rotationValue > buttonNotPressedValue)
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        // For smoother rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    public void HandleCameraZoom()
    {
        Vector3 temporaryZoom = cameraTranfrom.localPosition;
        float zoomValue = Input.GetAxis(zoomAxis);

        // Zoom in
        if (zoomValue < buttonNotPressedValue)
        {
            temporaryZoom += zoomAmount;
        }
        // Zoom out
        if (zoomValue > buttonNotPressedValue)
        {
            temporaryZoom -= zoomAmount;
        }

        // Limits the camera to a given level of zoom in the z axis. We use a Vector2 where the x maps to the minimum y and y maps to the maximum y.
        temporaryZoom.y = Mathf.Clamp(temporaryZoom.y, zoomLimit.x, zoomLimit.y);

        // For smoother zooming
        cameraTranfrom.localPosition = Vector3.Lerp(cameraTranfrom.localPosition, temporaryZoom, Time.deltaTime * movementTime);
    }
}
