using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public GameObject UnitSelect;
    RaycastHit hit;
    public List<GameObject> groupSelect = new List<GameObject>();

    void Update(){
        if (UnitSelect == null) {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift)) {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                    if (hit.transform.tag == "SelectableUnit") {
                        UnitSelect = hit.transform.gameObject;
                        UnitSelect.transform.Find("Marker").gameObject.SetActive(true);
                    }
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift)) {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
                    if (hit.transform.tag == "SelectableUnit") {
                        UnitSelect.transform.Find("Marker").gameObject.SetActive(false);
                        UnitSelect = hit.transform.gameObject;
                        UnitSelect.transform.Find("Marker").gameObject.SetActive(true);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
                if (hit.transform.tag == "SelectableUnit") {
                    if (UnitSelect != null){
                        groupSelect.Add(UnitSelect);
                        UnitSelect = null;
                        
                    }
                    groupSelect.Add(hit.transform.gameObject);

                    for(int i = 0; i < groupSelect.Count; i++){
                        groupSelect[i].transform.Find("Marker").gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
