using System.Collections.Generic;
using UnityEngine;

/**
 * Taken from!
 * https://github.com/danielmccluskey/A-Star-Pathfinding-Tutorial
 */
public class Pathfinder : MonoBehaviour
{

    GridGenerator grid;//For referencing the grid class

    private void Awake()//When the program starts
    {
        grid = GetComponent<GridGenerator>();//Get a reference to the game manager
    }

    private void Update()//Every frame
    {

    }

    public List<Node> findPath(Vector3 startPos, Vector3 targetPos, int pruneDistance)
    {
        Node startingNode = grid.getNodeByWorldPos(startPos);//Gets the node closest to the starting position
        Node endNode = grid.getNodeByWorldPos(targetPos);//Gets the node closest to the target position

        List<Node> openList = new List<Node>();//List of nodes for the open list
        HashSet<Node> passedList = new HashSet<Node>();//Hashset of nodes for the closed list
        List<Node> finalPath = null;

        openList.Add(startingNode);//Add the starting node to the open list to begin the program

        while (openList.Count > 0)//Whilst there is something in the open list
        {
            Node curNode = openList[0];//Create a node and set it to the first item in the open list
            for (int i = 1; i < openList.Count; i++)//Loop through the open list starting from the second object
            {
                if (openList[i].FCost < curNode.FCost || openList[i].FCost == curNode.FCost && openList[i].hCost < curNode.hCost)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    curNode = openList[i];//Set the current node to that object
                }
            }
            openList.Remove(curNode);//Remove that from the open list
            passedList.Add(curNode);//And add it to the closed list

            if (curNode == endNode)//If the current node is the same as the target node
            {
                finalPath = getFinalPath(startingNode, endNode, pruneDistance);//Calculate the final path
                break;
            }

            foreach (Node neighbourNode in grid.getNeighboringNodes(curNode))//Loop through each neighbor of the current node
            {
                if (!neighbourNode.isWall || passedList.Contains(neighbourNode))//If the neighbor is a wall or has already been checked
                {
                    continue; //Skip it
                }
                int MoveCost = curNode.gCost + getManhattenDistance(curNode, neighbourNode);//Get the F cost of that neighbor

                if (MoveCost < neighbourNode.gCost || !openList.Contains(neighbourNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    neighbourNode.gCost = MoveCost;//Set the g cost to the f cost
                    neighbourNode.hCost = getManhattenDistance(neighbourNode, endNode);//Set the h cost
                    neighbourNode.parentNode = curNode;//Set the parent of the node for retracing steps

                    if (!openList.Contains(neighbourNode))//If the neighbor is not in the openlist
                    {
                        openList.Add(neighbourNode);//Add it to the list
                    }
                }
            }
        }
        return finalPath;
    }

    List<Node> getFinalPath(Node a_StartingNode, Node a_EndNode, int pruneDistance)
    {
        List<Node> finalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            finalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.parentNode;//Move onto its parent node
        }

        finalPath.Reverse();//Reverse the path to get the correct order
        if (finalPath.Count - pruneDistance > 0)
        {
            finalPath.RemoveRange(finalPath.Count - pruneDistance, pruneDistance);
        } else
        {
            finalPath = null;
        }

        grid.finalPath = finalPath;//Set the final path
        return finalPath;
    }

    int getManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.gridPos.x - a_nodeB.gridPos.x); //x1-x2
        int iy = Mathf.Abs(a_nodeA.gridPos.y - a_nodeB.gridPos.y); //y1-y2

        return ix + iy;//Return the sum
    }
}
