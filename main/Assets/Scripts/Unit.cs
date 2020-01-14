using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    // short cut UnitSelect file as selectunit
    UnitSelection selectunit;
    UnityEngine.AI.NavMeshAgent agent;
    
    RaycastHit hit;

    void Start() {
        selectunit = GameObject.Find("UnitManager").GetComponent<UnitSelection>();
        agent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update() {
        if (selectunit) {
            if (selectunit.UnitSelect == this.gameObject || selectunit.groupSelect.Contains(this.gameObject)) {
                Debug.Log("Got through the gameobject!");
                if (Input.GetMouseButtonDown(1)) {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
                        if (hit.transform.tag == "Floor") {
                            agent.destination = hit.point;
                        }
                    }
                }
            }
        }
        
    }
}
