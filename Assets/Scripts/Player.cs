using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    private GameObject objectOfInterest = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.addListener(Events.MOVE_TO, moveTo);
    }

    // Update is called once per frame
    void Update()
    {
        /** Probably not be used until you figure out how it'll go
         * Either A* Pathfinding using nodes for the best way to not get stuck
         * Rotate around thing KH style perhaps
         * Lerping towards something always has some trouble though
         */
        if (objectOfInterest != null)
        {
            // Determine which direction to rotate towards
            Vector3 targetDirection = objectOfInterest.transform.position - transform.position;
            targetDirection.y = 0;
            Quaternion newDirection = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, moveSpeed * Time.deltaTime, 0.0f));
            if (transform.rotation == newDirection)
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            } else
            {
                transform.rotation = newDirection;
            }

            if (Vector2.Distance(new Vector2(objectOfInterest.transform.position.x, objectOfInterest.transform.position.z), new Vector2(transform.position.x, transform.position.z)) < 2.5f)
            {
                objectOfInterest = null;
            }

            if (Debug.isDebugBuild)
            {
                Debug.DrawRay(transform.position, transform.forward * 2.5f, Color.red);
            }
        }
    }

    void moveTo(Object n)
    {
        GameObject go = (GameObject)n;
        objectOfInterest = go;
    }
}
