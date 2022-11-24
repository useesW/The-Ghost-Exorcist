// Youtube Link Video Tutorial A* Pathfinding (E02: node grid) : https://www.youtube.com/watch?v=nhiFx28e7JY
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //public Transform player;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        // Subdivision of the gird based on the nodeDiameter
        nodeDiameter = nodeRadius * 2; 
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2; //Starting point

        for ( int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //offsetting from grid starting position based on grid subdivision
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours (Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Loop through local 3x3 grid  
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skips self
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    // Make sure neighbour grid position is within main grid
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY <gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
               
            }
        }
        return neighbours;
          
    }

    public List<Node> path;

    public Node NodeFromWolrldPoint (Vector3 worldPosition)
    {
        // Convert world position to percentage from left -> right and up -> down
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Use percentage to convert from world position to grid position (percent complete of grid bound)
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            //Node playerNode = NodeFromWolrldPoint(player.position);
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white: Color.red;
                if (path != null)
                {
                    if ( path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                //if (playerNode == n)
                //{
                //    Gizmos.color = Color.blue;
                //}
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
