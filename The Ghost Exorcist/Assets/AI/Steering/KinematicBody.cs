using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KinematicBody : MonoBehaviour
{

    [SerializeField] Transform targetObject;
    
    //Variables
    public float maxSpeed;
    public float targetBound;

    public float DistanceToPlayer()
    {
        float dist = Vector3.Distance(transform.position, targetObject.position);
        return dist;
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        //Move towards targeObject
        if (DistanceToPlayer() > targetBound)
        {
            transform.position += (targetObject.position - transform.position).normalized * maxSpeed;
        }

        transform.LookAt(targetObject); 
    }

    void Update()
    {
        
    }
}



