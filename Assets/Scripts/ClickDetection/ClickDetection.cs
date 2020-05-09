using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //EventManager.addListener(Events.CLICK, blep);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                EventManager.triggerEvent(Events.MOVE_TO, hit.transform.gameObject);
            }
        }
    }

    void blep(UnityEngine.Object obj)
    {
        Debug.Log("blep " + obj);
    }
}
