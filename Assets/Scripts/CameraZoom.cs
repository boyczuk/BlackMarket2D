using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public float panSpeed = 20f; // Speed at which the camera pans
    public Vector2 panLimit = new Vector2(50f, 50f); // Limit for camera panning

    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            mainCamera.orthographicSize = Mathf.Clamp(
                mainCamera.orthographicSize - scrollInput * zoomSpeed,
                minZoom,
                maxZoom
            );
        }
    }

    void HandlePan()
    {
        Vector3 pos = mainCamera.transform.position;

        // Pan with arrow keys or WASD
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("W key pressed");
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("S key pressed");
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("A key pressed");
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("D key pressed");
            pos.x += panSpeed * Time.deltaTime;
        }

        // Update the camera position
        mainCamera.transform.position = pos;

        // Debugging to check if the camera position is updated
        Debug.Log("Updated Camera Position: " + mainCamera.transform.position);
    }
}
