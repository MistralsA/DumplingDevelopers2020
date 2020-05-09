using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float turnAngleThreshold = 1f;
    public float distanceToNextNode = 0.1f;
    List<Node> pathway;
    int currentPathNode = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.addListener(Events.MOVE_TO, moveTo);
    }

    // Update is called once per frame
    void Update()
    {
        /**
         * Turning is being a problem
         * Currently turns KH speed but diagonals is being a problem.
         */
        if (pathway != null && pathway.Count > 0)
        {
            if (currentPathNode < pathway.Count) {
                Node objectOfInterest = pathway[currentPathNode];
                // Determine which direction to rotate towards
                Vector3 targetDirection = objectOfInterest.worldPosition - transform.position;
                targetDirection.y = 0;
                Quaternion newDirection = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, moveSpeed * Time.deltaTime, 0.0f));
                if (transform.rotation == newDirection)
                {
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;
                }
                else
                {
                    transform.rotation = newDirection;
                }

                if (Vector2.Distance(new Vector2(objectOfInterest.worldPosition.x, objectOfInterest.worldPosition.z), new Vector2(transform.position.x, transform.position.z)) < distanceToNextNode)
                {
                    currentPathNode++;
                }

                if (Debug.isDebugBuild)
                {
                    Debug.DrawRay(transform.position, transform.forward * 2.5f, Color.red);
                }
            } else
            {
                pathway = null;
            }
        }
        
    }

    void moveTo(Object n)
    {
        GameObject go = (GameObject) n;
        GameObject ga = GameObject.Find("GridManager");
        if (ga != null)
        {
            Pathfinder pa = ga.GetComponent<Pathfinder>();
            if (pa != null)
            {
                currentPathNode = 0;
                pathway = pa.findPath(transform.position, go.transform.position, 2);
            }
        }
    }
}
