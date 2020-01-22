using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This scripts focuses on the core control for the game
// Credit goes to UnityChat for the guide
public class RescpMouseMC : MonoBehaviour
{
     RaycastHit hit;

    // Clicking zones information
    public static float clickZone = 1.3f;
    private Vector3 pointOfMouseClick;  // captures hit.point
    private Vector2 pointOfMouseDrag;   // captures input.mousePosition

    // Mnit variables
    public static GameObject currentlySelectedUnit;
    public GameObject target;
    public static ArrayList currentlySelectedUnits = new ArrayList();

    // Mouse hold specifications
    public static float holdTimeReq = 1f;
    public float mouseHoldTimer = 0.0f;
    public bool userIsDragging = false;
    
    void Update()
    {
        // This might not be as optimized by having it continuously collect data, but it will allow is to differentiate between some core fucntions
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) {
            mouseDragRegister();

            // Allows us to misclick on terrain
            if(hit.collider.name == "Floor") {
                if(Input.GetMouseButtonUp(0) && clickZoneRegister(pointOfMouseClick)) {
                    if(!twinKeysPressed()) {
                        disbandUnits();
                    }
                }
            }

            // Clicking on units or outside of the game world
            else {
                if(Input.GetMouseButtonUp(0) && clickZoneRegister(pointOfMouseClick)) {
                    if (hit.collider.transform.Find("Selected")) {
                        
                        if(checkActiveUnits(hit.collider.gameObject)) {
                            if(twinKeysPressed()) {
                                removeUnitFromGroup(hit.collider.gameObject);
                            }
                            else {
                                // deselect all units and only selecting the new unti.
                                disbandUnits();

                                currentlySelectedUnits.Add(hit.collider.gameObject);
                                GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                selectedObject.SetActive(true);
                                Debug.Log("Added! 1");
                            }
                        }
                        else {
                            Debug.Log("triggered");
                            // deselect if we clicked on the group or any invalid objects
                            if(!twinKeysPressed()) {
                                disbandUnits();
                            }

                            // but on valid objects with specified tag, we add it.
                            currentlySelectedUnits.Add(hit.collider.gameObject);
                            GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                            selectedObject.SetActive(true);
                            Debug.Log("Added! 2");
                        }
                    }
                    else {
                        if(!twinKeysPressed()) {
                            disbandUnits();
                        }
                    }
                }
            }
        }
        // Clicked outside of the terrain
        else {
            if (Input.GetMouseButtonUp(0) && clickZoneRegister(pointOfMouseClick)) {
                if (!twinKeysPressed()) {
                    disbandUnits();
                }
            }
        }
    }

    // Registering two keys as the same input
    public static bool twinKeysPressed() {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            return true;
        }
        else {
            return false;
        }
    }

    // Detecting whether this is a drag or a click
    public void mouseDragRegister() {

        // On instance of click registered
        if(Input.GetMouseButtonDown(0)) {

            // We want to have accuracy and precission when click on the units, so we need world space.
            pointOfMouseClick = hit.point;

            // Dragging should be broad to group units, so we need screen space
            pointOfMouseDrag = Input.mousePosition;  

            // timer is on a incremental bases
            mouseHoldTimer = 0f;
        }

        // As click is being held down
        if(Input.GetMouseButton(0)) {
            if(!userIsDragging) {
                mouseHoldTimer += Time.deltaTime;
                if(mouseHoldTimer >= holdTimeReq || dragByPosition(pointOfMouseDrag, Input.mousePosition)) {
                    userIsDragging = true;
                }
            }
            else if(userIsDragging) {
                // Debug.Log("User is dragging");
            }
        }
        if(Input.GetMouseButtonUp(0)) {
            mouseHoldTimer = 0f;
            userIsDragging = false;
        }
    }
    // If there's a problem with drag select, check Vector 3
    public static bool dragByPosition(Vector2 startingPoint, Vector2 endingPoint) {

        // Here we compare the magnitude of the difference between the zones
        if((startingPoint.x > endingPoint.x + clickZone || startingPoint.x < endingPoint.x - clickZone) ||
            (startingPoint.y > endingPoint.y + clickZone || startingPoint.y < endingPoint.y - clickZone)) {
                return true;
        }
        else {
            return false;
        }
    }
    // A helper tool to make clicking untis more convinent by...
    public bool clickZoneRegister (Vector3 hitPoint) {
        if ((pointOfMouseClick.x < hitPoint.x + clickZone && pointOfMouseClick.x > hitPoint.x - clickZone) &&
            (pointOfMouseClick.y < hitPoint.y + clickZone && pointOfMouseClick.y > hitPoint.y - clickZone) &&
            (pointOfMouseClick.z < hitPoint.z + clickZone && pointOfMouseClick.z > hitPoint.z - clickZone)  ) {
            // Debug.Log(pointOfMouseClick.x + ":" + hitPoint.x + clickZone);
            return true;
        }
        else {
            return false;
        }
    }

    public void disbandUnits() {

        // toggling off all the markers
        for(int i = 0; i < currentlySelectedUnits.Count; i++) {
            GameObject temp = currentlySelectedUnits[i] as GameObject;
            temp.transform.Find("Selected").gameObject.SetActive(false);
        }
        currentlySelectedUnits.Clear();
    }

    // Check if the units is already active in the group
    public static bool checkActiveUnits (GameObject unit) {
        Debug.Log(currentlySelectedUnits.Count);
        if (currentlySelectedUnits.Count > 0) {
            for (int i = 0; i < currentlySelectedUnits.Count; i++){
                GameObject temp = currentlySelectedUnits[i] as GameObject;
                if (temp == unit) {
                    Debug.Log("Returned true");
                    return true;
                }
            }
            return false;
        }
        else {
            return false;
        }
    }

    // Remove individual units from the group
    public void removeUnitFromGroup (GameObject unit) {
        if (currentlySelectedUnits.Count > 0) {
            for (int i = 0; i < currentlySelectedUnits.Count; i++){
                GameObject temp = currentlySelectedUnits[i] as GameObject;
                if (temp == unit) {
                    currentlySelectedUnits.RemoveAt(i);
                    temp.transform.Find("Selected").gameObject.SetActive(false);
                }
            }
        }
    }


}
