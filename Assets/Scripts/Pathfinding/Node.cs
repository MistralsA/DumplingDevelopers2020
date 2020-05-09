using UnityEngine;

/**
 * Taken from!
 * https://github.com/danielmccluskey/A-Star-Pathfinding-Tutorial
 */
public class Node
{
    public Vector2Int gridPos;

    public bool isWall;
    public Vector3 worldPosition;

    public Node parentNode;

    public int gCost; //The cost of moving to the next square.
    public int hCost; //The distance to the goal from this node.

    public int FCost { get { return gCost + hCost; } } //Quick get function to add G cost and H Cost, and since we'll never need to edit FCost, we dont need a set function.

    public Node(bool _isWall, Vector3 _worldPos, int givenGridX, int givenGridY)
    {
        isWall = _isWall; //Tells the program if this node is being obstructed.
        worldPosition = _worldPos; //The world position of the node.
        gridPos = new Vector2Int(givenGridX, givenGridY);
    }
}
