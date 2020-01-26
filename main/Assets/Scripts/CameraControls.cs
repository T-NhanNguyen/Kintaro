using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private float panSpeed = 10f;
    private float panSpeedMultiplier = 2f;
    private float directionalSpeed;
    private float scrollSpeed = 400f;

    // units is in pixels
    public float panBorderThickness = 10f;

    public Vector3 camPos;

    public Vector3 camAngle;
    private float camRotSpeed = 40f;
    // units are in degrees
    private float maxVertRot = 65f;
    private float minVertRot = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;

    private float camZoom;
    private float maxZoomLimit = 20f;
    private float minZoomLimit = 5f;

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {
        handleMouseRotation();
        cameraMovement();

        //handled in handleMouseRotation()
        mouseX = Input.mousePosition.x;
        mouseY = Input.mousePosition.y;
    }   

    public void cameraMovement() {

        // Camera acceleration by WASD
        if(shiftKeysPressed()) {
            transform.position += transform.forward * (panSpeed * panSpeedMultiplier) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (panSpeed * panSpeedMultiplier) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else {
            transform.position += transform.forward * panSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * panSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        
        // Camera movement by mouse
        // Up
        if((Input.mousePosition.y > (Screen.height - panBorderThickness)) && !Input.GetMouseButton(2)) {
            if(shiftKeysPressed()) {
                transform.position += transform.forward * (panSpeed * panSpeedMultiplier) * Time.deltaTime;
            }
            else {
                transform.position += transform.forward * panSpeed * Time.deltaTime;
            }
            // Debug.Log("up");
        }

        // Down
        if((Input.mousePosition.y < (panBorderThickness)) && !Input.GetMouseButton(2)) {
            if(shiftKeysPressed()) {
                transform.position -= transform.forward * (panSpeed * panSpeedMultiplier) * Time.deltaTime;
            }
            else {
                transform.position -= transform.forward * panSpeed * Time.deltaTime;
            }
            // Debug.Log("down");
        }
        
        // Left
        if((Input.mousePosition.x < panBorderThickness) && !Input.GetMouseButton(2)) {
            if(shiftKeysPressed()) {
                transform.position -= transform.right * (panSpeed * panSpeedMultiplier) * Time.deltaTime;
            }
            else {
                transform.position -= transform.right * panSpeed * Time.deltaTime;
            }
            // Debug.Log("left");
        }

        // Right
        if((Input.mousePosition.x > (Screen.width - panBorderThickness)) && !Input.GetMouseButton(2)) {
            if(shiftKeysPressed()) {
                transform.position += transform.right * (panSpeed * panSpeedMultiplier) * Time.deltaTime;
            }
            else {
                transform.position += transform.right * panSpeed * Time.deltaTime;
            }
            // Debug.Log("Right");
        }

        // Camera Zoom opperation
        if(Input.GetAxis("Mouse ScrollWheel") != 0f) {
            transform.position += transform.up * scrollSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

            // Clamp camera zoom
            if(transform.position.y >= 20) {
                transform.position = new Vector3(transform.position.x, 20, transform.position.z);
            }
            if(transform.position.y <= 4) {
                transform.position = new Vector3(transform.position.x, 4, transform.position.z);
            }
        }
    }

    public void handleMouseRotation() {

        // set condition within cameraPan() to be !(middle mouse Click)
        if(Input.GetMouseButton(2) || Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl)) {

            // look along the horizontal axis
            if(Input.mousePosition.x != mouseX){
                
                // we have to rotate the controller and not the camera because the rotation will get messed up
                var cameraRotationY = (Input.mousePosition.x - mouseX) * camRotSpeed * Time.deltaTime;
                this.transform.Rotate(0, cameraRotationY, 0);
            }

            // look along the vertical axis
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

    public bool shiftKeysPressed() {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            return true;
        }
        else {
            return false;
        }
    }

    /*public void cameraPan() {
        camPos = transform.localPosition;

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
    }*/
}