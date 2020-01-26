using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingscripts : MonoBehaviour {
    RaycastHit hitResults;

    public static float clickZone = 1.3f;
    // private static Vector3 pointOfMouseClick;   // captures hitResults[0].point. Obsolete, only used in clickZoneRegister()
    private static Vector3 pointOfMouseDrag;    // captures input.mousePosition
    private static Vector3 pointOfMouseDown;

    // Mnit variables
    public GameObject target;
    public static ArrayList currentlySelectedUnits = new ArrayList();

    // Mouse hold specifications
    public static float holdTimeReq = 1f;
    public float mouseHoldTimer = 0.0f;
    public bool userIsDragging = false;

    // GUI variables
    public GUIStyle mouseDragSkin;
    private float boxWidth, boxHeight;
    private float boxLeft, boxTop;
    private Vector2 boxStart, boxFinish;
    

    void Update() {
        mouseDragRegister();

        if(Input.GetMouseButtonUp(0) && clickZoneRegister(pointOfMouseDrag)) {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitResults, Mathf.Infinity) && (hitResults.collider.gameObject.CompareTag("Floor"))) {
                if(!shiftKeysPressed()) {
                    // Debug.Log("Floor");
                    disbandUnits();
                }
            }
            else if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitResults, Mathf.Infinity) && (hitResults.collider.gameObject.CompareTag("SelectableUnit"))) {

                // If the unit is within the group, add it or reform group.
                if(checkActiveUnits(hitResults.collider.gameObject)) {
                    if(!shiftKeysPressed()) {
                        
                        // deselect all units and only selecting the new unti.
                        disbandUnits();

                        currentlySelectedUnits.Add(hitResults.collider.gameObject);
                        hitResults.collider.transform.Find("Selected").gameObject.SetActive(true);

                        // enabling units command script
                        hitResults.collider.GetComponent<UnitProperties>().enabled = true; 
                    }
                    else {
                        removeUnitFromGroup(hitResults.collider.gameObject);
                    }
                }
                else {
                    
                    // deselect if we clicked on the group or any invalid objects
                    if(!shiftKeysPressed()) {
                        disbandUnits();
                    }

                    // but on valid objects with specified tag, we add it.
                    currentlySelectedUnits.Add(hitResults.collider.gameObject);
                    GameObject activeMarker = hitResults.collider.transform.Find("Selected").gameObject;
                    activeMarker.SetActive(true);

                    hitResults.collider.GetComponent<UnitProperties>().enabled = true;            
                }
            }

            // anything else we click that isn't the terrain or unit
            else {
                if(!shiftKeysPressed()) {
                    disbandUnits();
                }
            }
        }
        drawBox();
    }

    // Detecting whether this is a drag or a click
    public void mouseDragRegister() {

        // On instance of click registered
        if(Input.GetMouseButtonDown(0)) {
            pointOfMouseDrag = Input.mousePosition;  
            pointOfMouseDown = hitResults.point;

            // timer is on a incremental basis
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
                currentMousePoint = Input.mousePosition;
                // Debug.Log("User is dragging, " + currentMousePoint);
                // select unit in dragbox function
            }
        }
        if(Input.GetMouseButtonUp(0)) {
            mouseHoldTimer = 0f;
            userIsDragging = false;
        }
    }
    
    // If there's a problem with drag select, check Vector 3
    public static bool dragByPosition(Vector3 startingPoint, Vector3 endingPoint) {

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
        if ((pointOfMouseDrag.x < hitPoint.x + clickZone && pointOfMouseDrag.x > hitPoint.x - clickZone) &&
            (pointOfMouseDrag.y < hitPoint.y + clickZone && pointOfMouseDrag.y > hitPoint.y - clickZone) &&
            (pointOfMouseDrag.z < hitPoint.z + clickZone && pointOfMouseDrag.z > hitPoint.z - clickZone)  ) {
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
            
            // turn on unit commands
            temp.GetComponent<UnitProperties>().enabled = false;
        }
        currentlySelectedUnits.Clear();
    }

    // Check if the units is already active in the group
    public static bool checkActiveUnits (GameObject unit) {
        // Debug.Log(currentlySelectedUnits.Count);
        if (currentlySelectedUnits.Count > 0) {
            for (int i = 0; i < currentlySelectedUnits.Count; i++){
                GameObject temp = currentlySelectedUnits[i] as GameObject;
                if (temp == unit) {
                    // Debug.Log("Returned true");
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

                    // turn on unit commands
                    temp.GetComponent<UnitProperties>().enabled = false;
                }
            }
        }
    }

    // Registering two keys as the same input
    public static bool shiftKeysPressed() {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            return true;
        }
        else {
            return false;
        }
    }
}
