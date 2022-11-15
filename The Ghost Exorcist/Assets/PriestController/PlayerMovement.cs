using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    //needs discussion, essentially how the player walks
    public float speed = 12f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //creates movement based off the player position via keys
        Vector3 move = transform.right * x + transform.forward * z;

        //function which calls the vector3 move code and applies it
        //controller.Move(move);

        //this multiply's the regular movement by how fast we set speed, needs discussion and then testing
        controller.Move(move * speed * Time.deltaTime);
    }
}
