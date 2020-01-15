using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMasterControl : MonoBehaviour
{
    RaycastHit hit;

    // or ClickDragZone
    public float clickZone = 1.3f;
    public static GameObject CurrentlySelectedUnit;
    public GameObject Target;

    // point in 3D space
    private Vector3 pointOfMouseClick;

    public static ArrayList CurrentlySelectedUnits = new ArrayList();

    // time we're measuring when the mouse click is being held.
    float holdTime = 0f;
    // minimum value to be considered a hold
    public static float minHoldTime = .3f;


    void Awake () {
        pointOfMouseClick = Vector3.zero;
    }
    void Update () {
        // Don't know if this method is more optimized than to scan after click. Revise later.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) {
            
            // storing point when clicked. for helper functions.
            if (Input.GetMouseButtonDown(0)) {
                pointOfMouseClick = hit.point;
                // Debug.Log("Clicked!");
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

        checkMouseStatus();
    }

    // Region Helper Function. Returns a validity check on our location of click
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

    // Comparison between Click-hold start and end pos. 
    // orig var name: UserDraggingByPosition
    public bool checkDraggingByPosition (Vector2 startPoint, Vector2 newPoint) {
        if (newPoint.x > startPoint.x + clickZone || newPoint.x < startPoint.x - clickZone ||
            newPoint.y > startPoint.y + clickZone || newPoint.y < startPoint.y - clickZone) {
            
            return true;
        }
        else {
            return false;
        }
    }

    // Comparison between click and hold
    public bool checkMouseStatus () {
        if (Input.GetMouseButton(0)) {
            holdTime += Time.deltaTime;
            // Debug.Log(holdTime);
        }

        // stopwatch did not meet min time needed.
        else if (Input.GetMouseButtonUp(0) && holdTime < minHoldTime) {
            // Debug.Log("Click");
            holdTime = 0f;
            return false;
        }
        if (holdTime >= minHoldTime) {
            // Debug.Log("Held! " + holdTime);
            holdTime = 0f;
            return true;
        }
    }
}
