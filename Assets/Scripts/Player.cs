using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.Example;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float turnAngleThreshold = 1f;
    public float distanceToNextNode = 0.1f;
    public float interactionRadius = 2.0f;
    List<Node> pathway;
    int currentPathNode = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.addListener(Events.MOVE_TO, moveTo);
    }

    // Remove all player control when we're in dialogue
    bool actionCanBeDone()
    {
        return FindObjectOfType<DialogueRunner>().IsDialogueRunning == false;
    }

    public bool isPlayerCloseTo(GameObject n)
    {
        return (n.transform.position - transform.position).magnitude <= interactionRadius;
    }

    // Update is called once per frame
    void Update()
    {
        // Detect if we want to start a conversation
        if (Input.GetKeyDown(KeyCode.Space) && actionCanBeDone())
        {
            CheckForNearbyNPC();
        }

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
        if (!actionCanBeDone()) { return; }
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

    /** Filter them to those that have a Yarn start node and are in range; 
     * then start a conversation with the first one
     */
    public void CheckForNearbyNPC()
    {
        var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
        var target = allParticipants.Find(delegate (NPC p) {
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null)
        {
            // Kick off the dialogue at this node.
            FindObjectOfType<DialogueRunner>().StartDialogue(target.talkToNode);
        }
    }

    /// Draw the range at which we'll start talking to people.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // Flatten the sphere into a disk, which looks nicer in 2D games
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 0));

        // Need to draw at position zero because we set position in the line above
        Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
    }
}
