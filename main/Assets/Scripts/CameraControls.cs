using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private float panSpeed = 10f;
    private float scrollSpeed = 20f;

    // units is in pixels
    public float panBorderThickness = 10f;

    public Vector3 camPos;

    public Vector3 camAngle;
    private float camRotSpeed = 40f;

    private float camZoom;
    private float maxZoomLimit = 20f;
    private float minZoomLimit = 5f;

    // Update is called once per frame
    void Update()
    {
        cameraPan();
        cameraRotate();
    }

    public void cameraPan() {
        camPos = transform.rotate;

        // Panning controls are available if not in rotating mode
        if(Input.GetKey("w") || (Input.mousePosition.y >= (Screen.height - panBorderThickness)) && !Input.GetMouseButton(2)) {

            // we want the speed to increase by the duration of key press and in relativity to time rather than to frames
            camPos.z += panSpeed *Time.deltaTime;
        }
        if(Input.GetKey("a") || (Input.mousePosition.x <= panBorderThickness) && !Input.GetMouseButton(2)) {
            camPos.x -= panSpeed *Time.deltaTime;
        }
        if(Input.GetKey("s") || (Input.mousePosition.y <= panBorderThickness) && !Input.GetMouseButton(2)) {
            camPos.z -= panSpeed *Time.deltaTime;
        }
        if(Input.GetKey("d") || (Input.mousePosition.x >= (Screen.width - panBorderThickness)) && !Input.GetMouseButton(2)) {
            camPos.x += panSpeed *Time.deltaTime;
        }

        // Camera Zoom
        camZoom = Input.GetAxis("Mouse ScrollWheel");
        camPos.y -= camZoom * scrollSpeed * Time.deltaTime * 100f;
        camPos.y = Mathf.Clamp(camPos.y, minZoomLimit, maxZoomLimit);

        transform.position = camPos;
    }
    public void cameraRotate() {
        camAngle = transform.rotation;
        if(Input.GetMouseButton(2)) {
            camAngle.x += camRotSpeed * Input.GetAxis("Mouse X");
            camAngle.y -= camRotSpeed * Input.GetAxis("Mouse y");
        }
        transform.eulerAngles = camAngle;
    }
}
