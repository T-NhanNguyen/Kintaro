using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMasterControl : MonoBehaviour
{
    RaycastHit hit;

    // or ClickDragZone
    public static float clickZone = 1.3f;
    private Vector3 pointOfMouseClick;  // captures hit.point
    private Vector2 pointOfMouseDrag;   // captures input.mousePoistion

    // unit variables
    public static GameObject CurrentlySelectedUnit;
    public GameObject Target;
    public static ArrayList CurrentlySelectedUnits = new ArrayList();

    // mouse hold specifications
    public static float holdTimeReq = 1f; // TimeLimitBeforeDelcareDrag
    public float mouseHoldTimer = 0.0f; // TimeLeftBeforeDeclareDrag;
    public bool userIsDragging = false;

    void Awake () {
        pointOfMouseClick = Vector3.zero;
    }
    void Update () {
        
            // Don't know if this method is more optimized than to scan after click. Revise later.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) {
            
            // storing point when clicked. for helper functions.
            if (Input.GetMouseButtonDown(0)) {
                pointOfMouseClick = hit.point;
                pointOfMouseDrag = Input.mousePosition;

                // this is a decrementing timer
                mouseHoldTimer = holdTimeReq;
            }
            if (Input.GetMouseButton(0)) {
                // Debug.Log(pointOfMouseClick + ":" + hit.point + ":" + clickZone);
                if (!userIsDragging) {
                    // Debug.Log("user is not dragging");
                    mouseHoldTimer -= Time.deltaTime;
                    if (mouseHoldTimer <= 0f || draggingByPos(pointOfMouseDrag, Input.mousePosition)) {
                        userIsDragging = true;
                    }
                }
                else if (userIsDragging) {
                    Debug.Log("User is dragging: " + mouseHoldTimer);
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                // Debug.Log("Button up");
                mouseHoldTimer = 0f;
                userIsDragging = false;
            }

            // To test location of click, uncomment below
            // Debug.Log(pointOfMouseClick);


            // Clicking on the terrain
            if (hit.collider.name == "Floor") {

                // Don't know what this feature does
                if(Input.GetMouseButtonUp(1)) {
                    GameObject TargetObj = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                    TargetObj.name = "Target Instantiated";
                    Debug.Log("Instantiate");
                }

                // Keeps the groups if clicked on terrain with the shift key pressed
                else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(pointOfMouseClick)){
                    if (!shiftKeysDown()) {
                        DeselectGameObject();
                    }
                }
            }

            else {

                // Did they click on a valid target?
                if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(pointOfMouseClick)) {
                    if (hit.collider.transform.Find("Selected")) {
                        if (!ActiveSelectedUnits(hit.collider.gameObject)) {
                            
                            // deactivate the group if shift is not pressed
                            if (!shiftKeysDown()) {
                                DeselectGameObject();
                            }
                                GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                selectedObject.SetActive(true);

                                CurrentlySelectedUnits.Add(hit.collider.gameObject);
                        }

                        // units are already within group
                        else {
                            if (shiftKeysDown()) {
                                removeUnitFromGroup(hit.collider.gameObject);
                            }
                            else {
                                DeselectGameObject();
                                GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                selectedObject.SetActive(true);

                                CurrentlySelectedUnits.Add(hit.collider.gameObject);
                            }
                        }
                    }

                    // If anything other than a selectable unit is selected
                    else {
                        if (!shiftKeysDown()) {
                            DeselectGameObject();
                        }
                    }
                }
            }
        }

        // Clicked outside of the terrain
        else {
            if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(pointOfMouseClick)) {
                if (!shiftKeysDown()) {
                    DeselectGameObject();
                }
            }
        }
    }

    // This section is for manual selection //
    // region Helper Function. Returns a validity check on our location of click
    public bool DidUserClickLeftMouse (Vector3 hitPoint) {
        if ((pointOfMouseClick.x < hitPoint.x + clickZone && pointOfMouseClick.x > hitPoint.x - clickZone) &&
            (pointOfMouseClick.y < hitPoint.y + clickZone && pointOfMouseClick.y > hitPoint.y - clickZone) &&
            (pointOfMouseClick.z < hitPoint.z + clickZone && pointOfMouseClick.z > hitPoint.z - clickZone)  ) {
            return true;
        }
        else {
            return false;
        }
    }

    // disband grouped units
    public static void DeselectGameObject () {
        if (CurrentlySelectedUnits.Count > 0) {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++){
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                ArrayListUnit.transform.Find("Selected").gameObject.SetActive(false);
            }
            
            CurrentlySelectedUnits.Clear();
        }
    }

    // original var name: UnitAlreadyInCurrentlySelectedUnits. Return true if we have active units
    public static bool ActiveSelectedUnits (GameObject Units) {
        if (CurrentlySelectedUnits.Count > 0) {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++){
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                if (ArrayListUnit == Units) {
                    return true;
                }
            }
            return false;
        }
        else {
            return false;
        }
    }

    public void removeUnitFromGroup (GameObject Unit) {
        if (CurrentlySelectedUnits.Count > 0) {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++){
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                if (ArrayListUnit == Unit) {
                    CurrentlySelectedUnits.RemoveAt(i);
                    ArrayListUnit.transform.Find("Selected").gameObject.SetActive(false);
                }
            }
        }
    }

    // merge both shifts for the same function. Return true for pressed
    public static bool shiftKeysDown () {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            return true;
        }
        else {
            return false;
        }
    }

    // This section is for drag box //
    // checking for mouse hold
    public bool draggingByPos (Vector2 startpoint, Vector2 newPoint) {

        // we're checking whether we should the magnitude of differences in mouse position is great enough to be considered as a box
        if ((newPoint.x > startpoint.x + clickZone || newPoint.x < startpoint.x - clickZone) ||
            (newPoint.y > startpoint.y + clickZone || newPoint.y < startpoint.y - clickZone)) {
            return true;
        }
        else {
            return false;
        }
    }
}
