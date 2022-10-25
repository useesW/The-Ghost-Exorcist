using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    float movementSpeed = 0.1f;
    Vector3 steer;

#region Obstacle Avoidence
    float obstacleCheckRange = 5.0f;
    LayerMask obstacleMask;
#endregion

    void Start() {
        steer = new Vector3(
            Random.Range(-movementSpeed,movementSpeed), 
            0.0f, 
            Random.Range(-movementSpeed,movementSpeed)).normalized;
    }

    void FixedUpdate() {
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

    public void Initialize(float speed, float oRange, LayerMask oMask){
        movementSpeed = speed;
        obstacleCheckRange = oRange;
        obstacleMask = oMask;
    }
#endregion

}
