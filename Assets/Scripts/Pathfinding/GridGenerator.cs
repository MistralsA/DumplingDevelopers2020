using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Taken from!
 * https://github.com/danielmccluskey/A-Star-Pathfinding-Tutorial
 */
public class GridGenerator : MonoBehaviour
{
    public Transform startPosition; // This is where the program will start the pathfinding from.
    public LayerMask wallLayer; // This is the mask that the program will look for when trying to find obstructions to the path.
    public Vector2 gridWorldSize; // A vector2 to store the width and height of the graph in world units.
    public float nodeRadius; // This stores how big each square on the graph will be
    public float nodePadding; // The distance that the squares will spawn from eachother.
    public bool avoidDiagonals = true; // Avoids making diagonals or not

    Node[,] allNodes; // The array of nodes that the A Star algorithm uses.
    public List<Node> finalPath; // The completed path that the red line will be drawn along

    float nodeDiameter; // Twice the amount of the radius (Set in the start function)
    Vector2Int gridSize;


    private void Start()//Ran once the program starts
    {
        nodeDiameter = nodeRadius * 2;//Double the radius to get diameter
        gridSize = new Vector2Int(Mathf.RoundToInt(gridWorldSize.x / nodeDiameter), Mathf.RoundToInt(gridWorldSize.y / nodeDiameter)); //Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        createGrid();//Draw the grid
    }

    void createGrid()
    {
        allNodes = new Node[gridSize.x, gridSize.y];//Declare the array of nodes.
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.
        for (int x = 0; x < gridSize.x; x++)//Loop through the array of nodes.
        {
            for (int y = 0; y < gridSize.y; y++)//Loop through the array of nodes
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);//Get the world co ordinates of the bottom left of the graph
                bool Wall = true; // Make the node a wall

                //If the node is not being obstructed
                //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
                //The if statement will return false.
                if (Physics.CheckSphere(worldPoint, nodeRadius, wallLayer))
                {
                    Wall = false;//Object is not a wall
                }

                allNodes[x, y] = new Node(Wall, worldPoint, x, y);//Create a new node in the array.
            }
        }
    }

    //Function that gets the neighboring nodes of the given node.
    public List<Node> getNeighboringNodes(Node curNode)
    {
        List<Node> neighbours = new List<Node>();//Make a new list of all available neighbors.

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //if we are on the node that was passed in, skip this iteration.
                if (x == 0 && y == 0 || (avoidDiagonals && Math.Abs(x) + Math.Abs(y) == 2))
                {
                    continue;
                }

                int checkX = curNode.gridPos.x + x;
                int checkY = curNode.gridPos.y + y;

                //Make sure the node is within the grid.
                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbours.Add(allNodes[checkX, checkY]); //Adds to the neighbours list.
                }

            }
        }

        return neighbours;//Return the neighbors list.
    }

    //Gets the closest node to the given world position.
    public Node getNodeByWorldPos(Vector3 worldPos)
    {
        float ixPos = ((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float iyPos = ((worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((gridSize.x - 1) * ixPos);
        int iy = Mathf.RoundToInt((gridSize.y - 1) * iyPos);

        return allNodes[ix, iy];
    }


    //Function that draws the wireframe
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));//Draw a wire cube with the given dimensions from the Unity inspector

        if (allNodes != null)//If the grid is not empty
        {
            foreach (Node n in allNodes)//Loop through every node in the grid
            {
                if (n.isWall)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.yellow;//Set the color of the node
                }

                if (finalPath != null)//If the final path is not empty
                {
                    if (finalPath.Contains(n))//If the current node is in the final path
                    {
                        Gizmos.color = Color.red;//Set the color of that node
                    }

                }


                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - nodePadding));//Draw the node at the position of the node.
            }
        }
    }
}
