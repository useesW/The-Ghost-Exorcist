// Youtube Link Video Tutorial A* Pathfinding (E03: algorithm implementation) : https://www.youtube.com/watch?v=mZfyt03LDH4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    public Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWolrldPoint(startPos);
        Node targetNode = grid.NodeFromWolrldPoint(targetPos);

        List<Node> openSet = new List<Node>(); 
        HashSet<Node> closedSet = new HashSet<Node>();  
        openSet.Add(startNode); // Start check at starting position

        // Loop unitl target node is found
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0]; 
            // Find node with lowest F cost -> evaluate node
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            // Move current node to closed set (now being evaluated)
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // Avoid checking non walkable nodes or nodes that have already been checked
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                // Calculate neighbour G cost
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // If faster g cost or node has not been evaluated
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;
                    
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

    }

    void RetracePath (Node startNode, Node endNode)
    {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);        

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
            
}
