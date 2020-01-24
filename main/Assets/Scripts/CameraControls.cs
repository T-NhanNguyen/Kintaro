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
    // units are in degrees
    private float maxVertRot = 65f;
    private float minVertRot = 0f;
    private float mouseX;
    private float mouseY;

    private float camZoom;
    private float maxZoomLimit = 20f;
    private float minZoomLimit = 5f;

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {
        handleMouseRotation();
        cameraPan();

        //handled in handleMouseRotation()
        mouseX = Input.mousePosition.x;
        mouseY = Input.mousePosition.y;
    }

    public void cameraPan() {
        camPos = transform.position;

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

        // Camera Zoom opperation
        camZoom = Input.GetAxis("Mouse ScrollWheel");
        camPos.y -= camZoom * scrollSpeed * Time.deltaTime * 100f;
        camPos.y = Mathf.Clamp(camPos.y, minZoomLimit, maxZoomLimit);

        transform.position = camPos;
    }

    public void handleMouseRotation() {

        // set condition within cameraPan() to be !(middle mouse Click)
        if(Input.GetMouseButton(2) || Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl)) {

            // if there's a change in the mouse, then we can change the rotation by the differences of current pos and previous location.
            if(Input.mousePosition.x != mouseX){
                var cameraRotationY = (Input.mousePosition.x - mouseX) * camRotSpeed * Time.deltaTime;
                this.transform.Rotate(0, cameraRotationY, 0);
            }
            if(Input.mousePosition.y != mouseY) {
                GameObject mainCamera = this.gameObject.transform.Find("Main Camera").gameObject;
                var cameraRotationX = (mouseY - Input.mousePosition.y) * camRotSpeed * Time.deltaTime;
                var desiredRotationX = mainCamera.transform.eulerAngles.x + cameraRotationX;

                if(desiredRotationX >= minVertRot && desiredRotationX <= maxVertRot) {
                    mainCamera.transform.Rotate(cameraRotationX, 0, 0);
                }
            }
        }
    }
}