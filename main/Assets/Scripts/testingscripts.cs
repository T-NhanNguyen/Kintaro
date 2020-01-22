using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingscripts : MonoBehaviour {

    // size of the array determines the maximum number of collateral results collected by these non-allocating methods. Eeach index of the array is the following object that's being hit . It will return an error if size == amount of objects to be collaterated
    // must user loops to go through all hitResults.collider.name
    // private static int rayCastIndexSize = 1;
    // RaycastHit[] hitResults = new RaycastHit[rayCastIndexSize];
    // (Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), hitResults, Mathf.Infinity) != 0)

    RaycastHit hitResults;


    public static float clickZone = 1.3f;
    // private static Vector3 pointOfMouseClick;   // captures hitResults[0].point. Obsolete, only used in clickZoneRegister()
    private static Vector3 pointOfMouseDrag;    // captures input.mousePosition
    private static Vector3 currentMousePoint;   // everyframes will store a point of world space into here.
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
    private float boxWidth;
    private float boxHeight;
    private float boxLeft, boxTop;
    private Vector2 boxStart;
    private Vector2 boxFinish;
    
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
                    GameObject selectedObject = hitResults.collider.transform.Find("Selected").gameObject;
                    selectedObject.SetActive(true);
                }
            }

            // anything else we click that isn't the terrain or unit
            else {
                if(!shiftKeysPressed()) {
                    disbandUnits();
                }
            }
        }
    }

    // display the GUI
    void OnGUI() {
        // box width, height, top, left
        if(userIsDragging) {
            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
        }
        
    }

    // Creates the dragbox
    private void drawDragBox() {
        boxWidth = Camera.main.WorldToScreenPoint(pointOfMouseDrag).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
        boxHeight = Camera.main.WorldToScreenPoint(pointOfMouseDrag).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;
        boxLeft = Input.mousePosition.x;
        // Debug.Log(boxWidth + " , " + boxHeight);
        Debug.Log(Camera.main.WorldToScreenPoint(pointOfMouseDrag).x + " - " + Camera.main.WorldToScreenPoint(currentMousePoint).x + " = " + boxWidth);
        //  inverting the axis for the space, so that the origin aligns between GUI and Screen
        boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;


        boxFinish = new Vector2(boxStart.x + Mathf.Abs(boxWidth), boxStart.y - Mathf.Abs(boxHeight));
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
                    drawDragBox();
                }
            }
            else if(userIsDragging) {
                currentMousePoint = Input.mousePosition;
                // Debug.Log("User is dragging, " + currentMousePoint);
                drawDragBox();
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
                Debug.Log(unit, temp);
                if (temp == unit) {
                    currentlySelectedUnits.RemoveAt(i);
                    temp.transform.Find("Selected").gameObject.SetActive(false);
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
