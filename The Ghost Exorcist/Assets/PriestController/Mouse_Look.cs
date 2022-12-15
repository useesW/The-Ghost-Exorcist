using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Look : MonoBehaviour
{
    //mouse sensitivty is how fast the mouse turns the camera, body is a function to lock it to the camera
    public float mouse_sensitivity = 100f;
    public Transform PlayerBody;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //locks the cursor to stay on screen and not go off to prevent weird camera turns
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //obtains the x and y location for the mouse while it moves and tracks
        float mouseX = Input.GetAxis("Mouse X") * mouse_sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouse_sensitivity * Time.deltaTime;

        //rotates the player by the mouse to look from x axis
        PlayerBody.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //rotates the player by mouse to look from y axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


    }
}
