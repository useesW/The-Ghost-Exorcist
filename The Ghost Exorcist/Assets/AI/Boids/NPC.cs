using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    [SerializeField] int ID;
    NPCManager manager;

#region Movement
    [Header("Movement Perameters")]
    [SerializeField] float movementSpeed = 0.1f;
    Vector3 steer;

#region Obstacle Avoidence
    [SerializeField] float obstacleCheckRange = 5.0f;
    LayerMask obstacleMask;
#endregion
#endregion

#region Ghost Values
     [Header("Ghost")]
     [SerializeField] bool drawPath;
     Color pathRenderColor;
     bool isGhost;
     [SerializeField] float pathCheckRange;
     [SerializeField] float pathFollowWeigth;
     [SerializeField] Material NPCMat;
     [SerializeField] Material ghostMat;

     List<Vector3> path;
     int pathIndex;
#endregion

    private void OnDrawGizmos() {
        if(drawPath && isGhost){
            foreach(Vector3 nodePos in path){
                Gizmos.color = pathRenderColor;
                Gizmos.DrawSphere(nodePos,0.3f);
            }
        }
    }

    void Start() {
        pathRenderColor = Random.ColorHSV(
                                            0.0f,1.0f,  // Hue
                                            0.9f,1.0f,  // Sat
                                            0.5f,1.0f,  // Val
                                            1.0f,1.0f); // Alpha

        steer = new Vector3(
            Random.Range(-movementSpeed,movementSpeed), 
            0.0f, 
            Random.Range(-movementSpeed,movementSpeed)).normalized;
    }

    void FixedUpdate() {
        if(isGhost){
            CalculateGhostPathSteer();
        }
        ObstacleCheck();
        SteerCorrection();
        transform.position += steer * movementSpeed; 
        transform.LookAt(transform.position + steer); // Taken From KinematicBody.cs
    }

    void ObstacleCheck(){
        RaycastHit hit;
        Vector3 collisonOffsetVector = Vector3.zero;
        for(int i = 0; i < 360; i += 90){
            Vector3 rotatedVector = Quaternion.Euler(0.0f,i,0.0f) * transform.forward;
            if(Physics.SphereCast(transform.position, 1.0f, rotatedVector, out hit, obstacleCheckRange, obstacleMask)){
                collisonOffsetVector += hit.normal * (obstacleCheckRange - hit.distance);
            }
        }
        steer = (steer + collisonOffsetVector).normalized;
    }

#region Halpers
    // Prevent Body From Flying (y > 0.0f)
    void SteerCorrection(){
        steer = new Vector3(steer.x, 0, steer.z);
    }

    public Vector3 GetSteer(){
        return steer;
    }

    public void SetSteer(Vector3 newSteer){
        steer = newSteer;
    }

    public void Initialize(int ID_, NPCManager manager_, float speed, float oRange, LayerMask oMask){
        ID = ID_;
        manager = manager_;
        movementSpeed = speed;
        obstacleCheckRange = oRange;
        obstacleMask = oMask;
        isGhost = false;
        path = new List<Vector3>();
    }

    public void AddignIsGhost(bool isGhost_){
        isGhost = isGhost_;
        if(isGhost){
            GetComponentInChildren<Renderer>().material = ghostMat;
            path = manager.GetPath(ID, false);
            pathIndex = 0;
        }else{
            GetComponentInChildren<Renderer>().material = NPCMat;
        }
    }

    void CalculateGhostPathSteer(){
        if(path.Count == 0) {return;}

        if(Vector3.Distance(transform.position, path[pathIndex]) < pathCheckRange){
            if(pathIndex == path.Count - 1){
                path = manager.GetPath(ID, true);
                pathIndex = 0;
            } else {
                pathIndex++;
            }
        }
        steer += pathFollowWeigth * (path[pathIndex] - transform.position).normalized;
        steer.Normalize();
    }
#endregion

}
