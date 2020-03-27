using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitProperties : MonoBehaviour
{
    NavMeshAgent agent;
    // size of the array determines the maximum number of collateral results collected by these non-allocating methods. Eeach index of the array is the following object that's being hit . It will return an error if size == amount of objects to be collaterated
    // must user loops to go through all hitResults.collider.name
    private static int rayCastIndexSize = 1;
    RaycastHit[] navHitResult = new RaycastHit[rayCastIndexSize];

    public float distance;
    public float followDistance;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1) && agent.enabled == true) {   
            if((Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), navHitResult, Mathf.Infinity) != 0)) {
                agent.SetDestination(navHitResult[0].point);
                Debug.Log("Move!");
            }
        }
    }

    void moveCommand() {
        
    }
}
