using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh : MonoBehaviour {

#region NavMesh Generation
    [Header("Nav Mesh")]
    [SerializeField] bool drawNavMesh;
    [SerializeField] LayerMask unwalkableMask;
    [SerializeField] Vector3 gridWorldSize;
    [SerializeField] float nodeRadius;
    float nodeDiameter;
    Node[,] grid;
    int gridSizeX, gridSizeY;
#endregion

    private void OnDrawGizmos() {
        if(drawNavMesh){
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if (grid != null) {
                foreach (Node n in grid) {
                    if(n.walkable){
                        Gizmos.color = new Color(50.0f,50.0f,50.0f,0.1f);
                        Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                    } else{
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                    }
                }
            }
        }
    }

    public void CreateGrid() {
        // Subdivision of the gird based on the nodeDiameter
        nodeDiameter = nodeRadius * 2; 
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        
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

    public Vector3 GetRandomWalkableNodePosition(){
        Vector3 gridPos = Vector3.zero;
        if(grid == null){
            Debug.LogError("Grid has not been created yet. Attempting to get absent grid position.");
        }

        Node n;
        do{
            n = grid[Random.Range(0, gridSizeX - 1),Random.Range(0,gridSizeY - 1)];
        } while (!n.walkable);
        gridPos = n.worldPosition;

        return gridPos;
    }

    public List<Node> GetNeighbours (Node node) {
        List<Node> neighbours = new List<Node>();

        // Loop through local 3x3 grid  
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                // Skips self
                if (x == 0 && y == 0) {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Make sure neighbour grid position is within main grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY <gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
          
    }

    public Node NodeFromWolrldPoint (Vector3 worldPosition) {
        // Convert world position to percentage from left -> right and up -> down
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Debug.Log(Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2)));
        // Use percentage to convert from world position to grid position (percent complete of grid bound)
        //float valX = (Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2)) - 1) * percentX;
        //float valY = (Mathf.RoundToInt(gridWorldSize.y / (nodeRadius * 2)) - 1) * percentY;
        //int x = Mathf.RoundToInt(valX);
        //int y = Mathf.RoundToInt(valY);
        
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
        return grid[x, y];
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos) {  
        Node startNode = NodeFromWolrldPoint(startPos);
        Node targetNode = NodeFromWolrldPoint(targetPos);

        List<Node> openSet = new List<Node>(); 
        HashSet<Node> closedSet = new HashSet<Node>();  
        openSet.Add(startNode); // Start check at starting position

        // Loop unitl target node is found
        while (openSet.Count > 0) {
            Node currentNode = openSet[0]; 
            // Find node with lowest F cost -> evaluate node
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || 
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            // Move current node to closed set (now being evaluated)
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode)) {
                // Avoid checking non walkable nodes or nodes that have already been checked
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }
                // Calculate neighbour G cost
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // If faster g cost or node has not been evaluated
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;
                    
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return new List<Vector3>();
    }

    List<Vector3> RetracePath (Node startNode, Node endNode) {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);        
        return (dstX > dstY)? (14 * dstY + 10 * (dstX - dstY)): (14 * dstX + 10 * (dstY - dstX));
    }
}
