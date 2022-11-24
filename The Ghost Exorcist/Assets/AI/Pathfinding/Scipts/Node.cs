// Youtube Link Video Tutorial A* Pathfinding (E01: algorithm explanation) : https://www.youtube.com/watch?v=-L-WgKMFuhE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable; 
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // distance from the starting node
    public int hCost; // distance from the ending node
    public Node Parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get{return gCost + hCost;}
    }

}
